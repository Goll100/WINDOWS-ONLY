using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace ScholasticaReader.Services
{
    public static class SecurityService
    {
        private const string MasterPassword = "ASHIRAF";
        private static readonly byte[] Key = Encoding.UTF8.GetBytes("SchoLast!c@R34d3rK3y"); // 24 bytes
        private static readonly byte[] IV = Encoding.UTF8.GetBytes("!n1tVect0r12");

        public static bool VerifyMasterPassword(string input) => input == MasterPassword;

        public static bool VerifyIntegrity()
        {
            string exePath = System.Reflection.Assembly.GetExecutingAssembly().Location;
            byte[] currentHash;
            using (var sha256 = SHA256.Create())
            using (var stream = File.OpenRead(exePath))
                currentHash = sha256.ComputeHash(stream);

            string storedHash = LoadStoredHash();
            return Convert.ToHexString(currentHash) == storedHash;
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
            using (TripleDESCryptoServiceProvider des = new TripleDESCryptoServiceProvider())
            {
                des.Key = Key;
                des.IV = IV;
                var encryptor = des.CreateEncryptor();
                byte[] plainBytes = Encoding.UTF8.GetBytes(plainText);
                byte[] cipherBytes = encryptor.TransformFinalBlock(plainBytes, 0, plainBytes.Length);
                return Convert.ToBase64String(cipherBytes);
            }
        }

        public static string DecryptString(string cipherText)
        {
            using (TripleDESCryptoServiceProvider des = new TripleDESCryptoServiceProvider())
            {
                des.Key = Key;
                des.IV = IV;
                var decryptor = des.CreateDecryptor();
                byte[] cipherBytes = Convert.FromBase64String(cipherText);
                byte[] plainBytes = decryptor.TransformFinalBlock(cipherBytes, 0, cipherBytes.Length);
                return Encoding.UTF8.GetString(plainBytes);
            }
        }
    }
} 
