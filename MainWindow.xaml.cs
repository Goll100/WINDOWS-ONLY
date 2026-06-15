using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows;
using Microsoft.Web.WebView2.Wpf;
using ScholasticaReader.Models;
using ScholasticaReader.Services;
using ScholasticaReader.Views;

namespace ScholasticaReader
{
    public partial class MainWindow : Window
    {
        private BookService bookService;
        private AnnotationService annotationService;
        private LicenseService license;
        private ObservableCollection<Book> library = new ObservableCollection<Book>();

        public MainWindow()
        {
            InitializeComponent();
            license = new LicenseService();
            bookService = new BookService();
            annotationService = new AnnotationService();
            LoadLibrary();
            InitializeWebView();

            if (!license.IsPremium)
            {
                TeacherDashboardBtn.IsEnabled = false;
                // Freemium restrictions can be added here
            }
        }

        private async void InitializeWebView()
        {
            await BookWebView.EnsureCoreWebView2Async(null);
            BookWebView.CoreWebView2.NavigateToString("<html><body><h1>Select a book from the library</h1></body></html>");
        }

        private void LoadLibrary()
        {
            library.Clear();
            var books = bookService.GetAllBooks();
            foreach (var b in books) library.Add(b);
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
                var ann = new Annotation
                {
                    BookId = book.Id,
                    Type = "Note",
                    Text = NoteTextBox.Text,
                    Page = 1,
                    CreatedAt = DateTime.Now
                };
                annotationService.AddAnnotation(ann);
                RefreshAnnotations(book.Id);
                NoteTextBox.Clear();
            }
        }

        private void Highlight_Click(object sender, RoutedEventArgs e)
        {
            if (LibraryListBox.SelectedItem is Book book)
            {
                var ann = new Annotation
                {
                    BookId = book.Id,
                    Type = "Highlight",
                    Text = "User selected highlight",
                    Page = 1,
                    CreatedAt = DateTime.Now
                };
                annotationService.AddAnnotation(ann);
                RefreshAnnotations(book.Id);
            }
        }

        private void RefreshAnnotations(int bookId)
        {
            var anns = annotationService.GetAnnotationsForBook(bookId);
            AnnotationsListBox.ItemsSource = anns;
        }

        private void TTS_Click(object sender, RoutedEventArgs e)
        {
            var synthesizer = new System.Speech.Synthesis.SpeechSynthesizer();
            synthesizer.SpeakAsync("Text to speech example. You would extract text from the book.");
        }

        private void Flashcards_Click(object sender, RoutedEventArgs e)
        {
            var flashWin = new FlashcardWindow();
            flashWin.ShowDialog();
        }

        private void MindMap_Click(object sender, RoutedEventArgs e)
        {
            var mindWin = new MindMapWindow();
            mindWin.ShowDialog();
        }

        private void TeacherDashboard_Click(object sender, RoutedEventArgs e)
        {
            if (license.IsPremium)
            {
                var dash = new TeacherDashboard();
                dash.ShowDialog();
            }
            else
            {
                MessageBox.Show("Teacher Dashboard is a premium feature.");
            }
        }

        private void ParallelReading_Click(object sender, RoutedEventArgs e)
        {
            var parallelWin = new ParallelReadingWindow();
            parallelWin.Show();
        }
    }
} 
