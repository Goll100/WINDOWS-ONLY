using System.Windows;
using ScholasticaReader.Services;

namespace ScholasticaReader
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            
            // Anti-tampering check
            if (!SecurityService.VerifyIntegrity())
            {
                MessageBox.Show("Application has been tampered with. Exiting.",
                    "Security Violation", MessageBoxButton.OK, MessageBoxImage.Error);
                Shutdown();
                return;
            }

            // Initialize license check
            var license = new LicenseService();
            if (!license.ValidateLicense())
            {
                Shutdown();
                return;
            }
        }
    }
} 
