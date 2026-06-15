using System;
using System.IO;
using Microsoft.Data.Sqlite;

namespace ScholasticaReader.Services
{
    public class LicenseService
    {
        private readonly string dbPath;
        private DateTime? licenceExpiry;
        private bool isPremium = false;

        public LicenseService()
        {
            dbPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "ScholasticaReader", "license.db");
            Directory.CreateDirectory(Path.GetDirectoryName(dbPath));
            InitializeDatabase();
            LoadLicenseStatus();
        }

        private void InitializeDatabase()
        {
            if (!File.Exists(dbPath))
            {
                using (var connection = new SqliteConnection($"Data Source={dbPath}"))
                {
                    connection.Open();
                    var command = connection.CreateCommand();
                    command.CommandText = @"
                        CREATE TABLE Licenses (
                            Id INTEGER PRIMARY KEY AUTOINCREMENT,
                            HWID TEXT NOT NULL,
                            ActivationCode TEXT NOT NULL,
                            ExpiryDate TEXT NOT NULL,
                            IsPremium INTEGER NOT NULL
                        );
                    ";
                    command.ExecuteNonQuery();
                }
            }
        }

        private void LoadLicenseStatus()
        {
            using (var connection = new SqliteConnection($"Data Source={dbPath}"))
            {
                connection.Open();
                var command = connection.CreateCommand();
                command.CommandText = "SELECT ExpiryDate, IsPremium FROM Licenses ORDER BY Id DESC LIMIT 1";
                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        licenceExpiry = DateTime.Parse(reader.GetString(0));
                        isPremium = reader.GetInt32(1) == 1;
                    }
                }
            }
        }

        public bool ValidateLicense()
        {
            if (licenceExpiry == null || licenceExpiry <= DateTime.Now)
            {
                return ShowActivationDialog();
            }
            return true;
        }

        private bool ShowActivationDialog()
        {
            string hwid = HWIDHelper.GetHWID();
            string message = $"Your Hardware ID (HWID):\n{hwid}\n\nSend this to the developer to get an activation code.\n\nEnter the code below:";
            string code = Microsoft.VisualBasic.Interaction.InputBox(message, "Activation", "");
            if (string.IsNullOrEmpty(code))
                return false;

            if (VerifyActivationCode(hwid, code))
            {
                SaveLicense(code);
                LoadLicenseStatus();
                return true;
            }
            else
            {
                MessageBox.Show("Invalid activation code or it has expired.", "Activation Failed");
                return false;
            }
        }

        private bool VerifyActivationCode(string hwid, string code)
        {
            try
            {
                string decrypted = SecurityService.DecryptString(code);
                var parts = decrypted.Split('|');
                if (parts.Length != 3) return false;
                if (parts[0] != hwid) return false;
                DateTime expiry = DateTime.Parse(parts[1]);
                if (expiry <= DateTime.Now) return false;
                // No HWID reuse check – allows weekly renewal with new codes
                return true;
            }
            catch
            {
                return false;
            }
        }

        private void SaveLicense(string code)
        {
            string decrypted = SecurityService.DecryptString(code);
            var parts = decrypted.Split('|');
            DateTime expiry = DateTime.Parse(parts[1]);
            bool premium = parts[2] == "Premium";

            using (var connection = new SqliteConnection($"Data Source={dbPath}"))
            {
                connection.Open();
                var cmd = connection.CreateCommand();
                cmd.CommandText = @"
                    INSERT INTO Licenses (HWID, ActivationCode, ExpiryDate, IsPremium)
                    VALUES ($hwid, $code, $expiry, $premium)
                ";
                cmd.Parameters.AddWithValue("$hwid", HWIDHelper.GetHWID());
                cmd.Parameters.AddWithValue("$code", code);
                cmd.Parameters.AddWithValue("$expiry", expiry.ToString("yyyy-MM-dd HH:mm:ss"));
                cmd.Parameters.AddWithValue("$premium", premium ? 1 : 0);
                cmd.ExecuteNonQuery();
            }
        }

        public bool IsPremium => isPremium;
        public DateTime? ExpiryDate => licenceExpiry;
    }
}
