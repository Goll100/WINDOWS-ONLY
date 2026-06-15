using System;
using System.Windows;
using ScholasticaReader.Services;

namespace ScholasticaReader
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            try
            {
                base.OnStartup(e);
                
                if (!SecurityService.VerifyIntegrity())
                {
                    MessageBox.Show("Tampering detected. Exiting application.", "Security Alert");
                    Shutdown(1);
                    return;
                }
                
                var license = new LicenseService();
                if (!license.ValidateLicense())
                {
                    MessageBox.Show("License validation failed. Application will exit.", "License Error");
                    Shutdown(1);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error during application startup: {ex.Message}", "Startup Error");
                Shutdown(1);
            }
        }
    }
}
