using System.Windows;

namespace ScholasticaReader.Views
{
    public partial class FlashcardWindow : Window
    {
        private string[] questions = { "What is the capital of France?", "What is 2+2?" };
        private string[] answers = { "Paris", "4" };
        private int index = 0;

        public FlashcardWindow()
        {
            InitializeComponent();
            ShowCurrent();
        }

        private void ShowCurrent()
        {
            QuestionText.Text = questions[index];
            AnswerText.Visibility = Visibility.Collapsed;
        }

        private void ShowAnswer_Click(object sender, RoutedEventArgs e)
        {
            AnswerText.Text = answers[index];
            AnswerText.Visibility = Visibility.Visible;
        }

        private void Easy_Click(object sender, RoutedEventArgs e)
        {
            index = (index + 1) % questions.Length;
            ShowCurrent();
        }

        private void Hard_Click(object sender, RoutedEventArgs e)
        {
            index = (index + 1) % questions.Length;
            ShowCurrent();
        }
    }
} 
