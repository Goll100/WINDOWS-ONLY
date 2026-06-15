using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace ScholasticaReader.Services
{
    public static class SecurityService
    {
        // NOTE: In production, these should be stored in secure configuration or environment variables
        private const string MasterPassword = "ASHIRAF";
        
        // AES-256 encryption key (32 bytes for 256-bit encryption)
        private static readonly byte[] Key = Encoding.UTF8.GetBytes("SchoLast!c@R34d3rK3ySecureKey12"); // 32 bytes
        
        public static bool VerifyMasterPassword(string input) => input == MasterPassword;

        public static bool VerifyIntegrity()
        {
            try
            {
                string exePath = System.Reflection.Assembly.GetExecutingAssembly().Location;
                byte[] currentHash;
                using (var sha256 = SHA256.Create())
                using (var stream = File.OpenRead(exePath))
                    currentHash = sha256.ComputeHash(stream);

                string storedHash = LoadStoredHash();
                return Convert.ToHexString(currentHash) == storedHash;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error verifying integrity: {ex.Message}");
                return false;
            }
        }

        private static string LoadStoredHash()
        {
            string hashFile = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "ScholasticaReader", "integrity.hash");
            if (File.Exists(hashFile))
                return File.ReadAllText(hashFile).Trim();
            
            string exePath = System.Reflection.Assembly.GetExecutingAssembly().Location;
            using (var sha256 = SHA256.Create())
            using (var stream = File.OpenRead(exePath))
            {
                string hash = Convert.ToHexString(sha256.ComputeHash(stream));
                Directory.CreateDirectory(Path.GetDirectoryName(hashFile));
                File.WriteAllText(hashFile, hash);
                return hash;
            }
        }

        public static string EncryptString(string plainText)
        {
            try
            {
                using (Aes aes = Aes.Create())
                {
                    aes.Key = Key;
                    aes.Mode = CipherMode.CBC;
                    aes.Padding = PaddingMode.PKCS7;
                    
                    // Generate a new random IV for each encryption
                    byte[] iv = aes.IV;
                    
                    using (var encryptor = aes.CreateEncryptor(aes.Key, iv))
                    {
                        byte[] plainBytes = Encoding.UTF8.GetBytes(plainText);
                        byte[] cipherBytes = encryptor.TransformFinalBlock(plainBytes, 0, plainBytes.Length);
                        
                        // Prepend IV to ciphertext
                        byte[] result = new byte[iv.Length + cipherBytes.Length];
                        Buffer.BlockCopy(iv, 0, result, 0, iv.Length);
                        Buffer.BlockCopy(cipherBytes, 0, result, iv.Length, cipherBytes.Length);
                        
                        return Convert.ToBase64String(result);
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error encrypting string: {ex.Message}");
                return string.Empty;
            }
        }

        public static string DecryptString(string cipherText)
        {
            try
            {
                byte[] buffer = Convert.FromBase64String(cipherText);
                
                using (Aes aes = Aes.Create())
                {
                    aes.Key = Key;
                    aes.Mode = CipherMode.CBC;
                    aes.Padding = PaddingMode.PKCS7;
                    
                    // Extract IV from the beginning of the buffer
                    byte[] iv = new byte[aes.IV.Length];
                    Buffer.BlockCopy(buffer, 0, iv, 0, iv.Length);
                    
                    byte[] cipherBytes = new byte[buffer.Length - iv.Length];
                    Buffer.BlockCopy(buffer, iv.Length, cipherBytes, 0, cipherBytes.Length);
                    
                    using (var decryptor = aes.CreateDecryptor(aes.Key, iv))
                    {
                        byte[] plainBytes = decryptor.TransformFinalBlock(cipherBytes, 0, cipherBytes.Length);
                        return Encoding.UTF8.GetString(plainBytes);
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error decrypting string: {ex.Message}");
                return string.Empty;
            }
        }
    }
}
