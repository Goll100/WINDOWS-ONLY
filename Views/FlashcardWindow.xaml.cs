using System;
using System.Windows;

namespace ScholasticaReader.Views
{
    public partial class FlashcardWindow : Window
    {
        public FlashcardWindow()
        {
            try
            {
                InitializeComponent();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error initializing flashcard window: {ex.Message}", "Initialization Error");
            }
        }
    }
}
