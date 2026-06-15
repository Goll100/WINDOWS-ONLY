using System.Windows;

namespace ScholasticaReader.Views
{
    public partial class TeacherDashboard : Window
    {
        public TeacherDashboard()
        {
            InitializeComponent();
            ProgressGrid.ItemsSource = new[] {
                new { Student = "Alice", Progress = "45%", LastActive = "2025-03-20" },
                new { Student = "Bob", Progress = "78%", LastActive = "2025-03-21" }
            };
        }

        private void AssignReading_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Assign reading feature saves to local database.");
        }
    }
}
