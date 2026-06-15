using System;
using System.Collections.ObjectModel;
using System.Windows;
using Microsoft.Web.WebView2.Wpf;
using ScholasticaReader.Models;
using ScholasticaReader.Services;
using ScholasticaReader.Views;

namespace ScholasticaReader
{
    public partial class MainWindow : Window
    {
        private BookService bookService = new BookService();
        private AnnotationService annotationService = new AnnotationService();
        private LicenseService license;
        private ObservableCollection<Book> library = new ObservableCollection<Book>();

        public MainWindow()
        {
            InitializeComponent();
            license = new LicenseService();
            LoadLibrary();
            InitializeWebView();
            if (!license.IsPremium) TeacherDashboardBtn.IsEnabled = false;
        }

        private async void InitializeWebView()
        {
            await BookWebView.EnsureCoreWebView2Async(null);
            BookWebView.CoreWebView2.NavigateToString("<html><body><h1>Select a book</h1></body></html>");
        }

        private void LoadLibrary()
        {
            library.Clear();
            foreach (var b in bookService.GetAllBooks()) library.Add(b);
            LibraryListBox.ItemsSource = library;
        }

        private async void OpenBook_Click(object sender, RoutedEventArgs e)
        {
            var selected = LibraryListBox.SelectedItem as Book;
            if (selected == null) return;
            string content = bookService.GetBookContent(selected.FilePath);
            await BookWebView.EnsureCoreWebView2Async(null);
            BookWebView.CoreWebView2.NavigateToString(content);
        }

        private void AddNote_Click(object sender, RoutedEventArgs e)
        {
            if (LibraryListBox.SelectedItem is Book book && !string.IsNullOrWhiteSpace(NoteTextBox.Text))
            {
                annotationService.AddAnnotation(new Annotation { BookId = book.Id, Type = "Note", Text = NoteTextBox.Text, Page = 1, CreatedAt = DateTime.Now });
                RefreshAnnotations(book.Id);
                NoteTextBox.Clear();
            }
        }

        private void Highlight_Click(object sender, RoutedEventArgs e)
        {
            if (LibraryListBox.SelectedItem is Book book)
            {
                annotationService.AddAnnotation(new Annotation { BookId = book.Id, Type = "Highlight", Text = "Highlight", Page = 1, CreatedAt = DateTime.Now });
                RefreshAnnotations(book.Id);
            }
        }

        private void RefreshAnnotations(int bookId) => AnnotationsListBox.ItemsSource = annotationService.GetAnnotationsForBook(bookId);
        private void TTS_Click(object sender, RoutedEventArgs e) => new System.Speech.Synthesis.SpeechSynthesizer().SpeakAsync("Text to speech sample.");
        private void Flashcards_Click(object sender, RoutedEventArgs e) => new FlashcardWindow().ShowDialog();
        private void MindMap_Click(object sender, RoutedEventArgs e) => new MindMapWindow().ShowDialog();
        private void TeacherDashboard_Click(object sender, RoutedEventArgs e)
        {
            if (license.IsPremium) new TeacherDashboard().ShowDialog();
            else MessageBox.Show("Premium feature.");
        }
        private void ParallelReading_Click(object sender, RoutedEventArgs e) => new ParallelReadingWindow().Show();
    }
}
