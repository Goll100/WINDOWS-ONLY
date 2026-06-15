using System.Windows;
using ScholasticaReader.Services;

namespace ScholasticaReader
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            if (!SecurityService.VerifyIntegrity())
            {
                MessageBox.Show("Tampering detected. Exiting.");
                Shutdown();
                return;
            }
            var license = new LicenseService();
            if (!license.ValidateLicense()) Shutdown();
        }
    }
}
