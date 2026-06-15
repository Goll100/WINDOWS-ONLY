using System;
using System.Windows;

namespace ScholasticaReader.Views
{
    public partial class TeacherDashboard : Window
    {
        public TeacherDashboard()
        {
            try
            {
                InitializeComponent();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error initializing teacher dashboard: {ex.Message}", "Initialization Error");
            }
        }
    }
}
