using System;
using System.Security.Cryptography;
using System.Text;
using System.Windows;

namespace ActivationGenerator
{
    public partial class MainWindow : Window
    {
        private static readonly byte[] Key = Encoding.UTF8.GetBytes("SchoLast!c@R34d3rK3y");
        private static readonly byte[] IV = Encoding.UTF8.GetBytes("!n1tVect0r12");

        public MainWindow() => InitializeComponent();

        private void Generate_Click(object sender, RoutedEventArgs e)
        {
            string hwid = HwidBox.Text.Trim();
            if (string.IsNullOrEmpty(hwid))
            {
                MessageBox.Show("Enter HWID");
                return;
            }
            if (!int.TryParse(DaysBox.Text, out int days) || days <= 0)
            {
                MessageBox.Show("Enter valid days");
                return;
            }
            string expiry = DateTime.Now.AddDays(days).ToString("yyyy-MM-dd HH:mm:ss");
            string premium = PremiumBox.IsChecked == true ? "Premium" : "Free";
            string plain = $"{hwid}|{expiry}|{premium}";

            using (TripleDESCryptoServiceProvider des = new TripleDESCryptoServiceProvider())
            {
                des.Key = Key;
                des.IV = IV;
                byte[] plainBytes = Encoding.UTF8.GetBytes(plain);
                byte[] cipherBytes = des.CreateEncryptor().TransformFinalBlock(plainBytes, 0, plainBytes.Length);
                CodeBox.Text = Convert.ToBase64String(cipherBytes);
            }
        }
    }
}
