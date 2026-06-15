using System;
using System.Windows;

namespace ScholasticaReader.Views
{
    public partial class MindMapWindow : Window
    {
        public MindMapWindow()
        {
            try
            {
                InitializeComponent();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error initializing mind map window: {ex.Message}", "Initialization Error");
            }
        }
    }
}
