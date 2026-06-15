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
        private int currentPage = 1;

        public MainWindow()
        {
            try
            {
                InitializeComponent();
                license = new LicenseService();
                LoadLibrary();
                InitializeWebView();
                if (!license.IsPremium) TeacherDashboardBtn.IsEnabled = false;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error initializing main window: {ex.Message}", "Initialization Error");
            }
        }

        private async void InitializeWebView()
        {
            try
            {
                await BookWebView.EnsureCoreWebView2Async(null);
                BookWebView.CoreWebView2.NavigateToString("<html><body><h1>Select a book</h1></body></html>");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error initializing WebView: {ex.Message}");
                MessageBox.Show($"Error initializing book viewer: {ex.Message}", "WebView Error");
            }
        }

        private void LoadLibrary()
        {
            try
            {
                library.Clear();
                foreach (var b in bookService.GetAllBooks())
                {
                    library.Add(b);
                }
                LibraryListBox.ItemsSource = library;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading library: {ex.Message}", "Library Error");
            }
        }

        private async void OpenBook_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var selected = LibraryListBox.SelectedItem as Book;
                if (selected == null)
                {
                    MessageBox.Show("Please select a book first.", "No Book Selected");
                    return;
                }

                string content = bookService.GetBookContent(selected.FilePath);
                await BookWebView.EnsureCoreWebView2Async(null);
                BookWebView.CoreWebView2.NavigateToString(content);
                currentPage = 1; // Reset page counter
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error opening book: {ex.Message}", "Open Book Error");
            }
        }

        private void AddNote_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (LibraryListBox.SelectedItem is Book book && !string.IsNullOrWhiteSpace(NoteTextBox.Text))
                {
                    annotationService.AddAnnotation(new Annotation 
                    { 
                        BookId = book.Id, 
                        Type = "Note", 
                        Text = NoteTextBox.Text, 
                        Page = currentPage, 
                        CreatedAt = DateTime.Now 
                    });
                    RefreshAnnotations(book.Id);
                    NoteTextBox.Clear();
                }
                else if (LibraryListBox.SelectedItem == null)
                {
                    MessageBox.Show("Please select a book first.", "No Book Selected");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error adding note: {ex.Message}", "Add Note Error");
            }
        }

        private void Highlight_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (LibraryListBox.SelectedItem is Book book)
                {
                    annotationService.AddAnnotation(new Annotation 
                    { 
                        BookId = book.Id, 
                        Type = "Highlight", 
                        Text = "Highlight", 
                        Page = currentPage, 
                        CreatedAt = DateTime.Now 
                    });
                    RefreshAnnotations(book.Id);
                }
                else
                {
                    MessageBox.Show("Please select a book first.", "No Book Selected");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error adding highlight: {ex.Message}", "Highlight Error");
            }
        }

        private void RefreshAnnotations(int bookId)
        {
            try
            {
                AnnotationsListBox.ItemsSource = annotationService.GetAnnotationsForBook(bookId);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error refreshing annotations: {ex.Message}");
            }
        }

        private void TTS_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (LibraryListBox.SelectedItem == null)
                {
                    MessageBox.Show("Please select a book first.", "No Book Selected");
                    return;
                }
                new System.Speech.Synthesis.SpeechSynthesizer().SpeakAsync("Text to speech sample.");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error with text-to-speech: {ex.Message}", "TTS Error");
            }
        }

        private void Flashcards_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var flashcardWindow = new FlashcardWindow();
                flashcardWindow.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error opening flashcards: {ex.Message}", "Flashcard Error");
            }
        }

        private void MindMap_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var mindMapWindow = new MindMapWindow();
                mindMapWindow.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error opening mind map: {ex.Message}", "Mind Map Error");
            }
        }

        private void TeacherDashboard_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (license.IsPremium)
                {
                    var dashboardWindow = new TeacherDashboard();
                    dashboardWindow.ShowDialog();
                }
                else
                {
                    MessageBox.Show("Premium feature. Please upgrade your license.", "Premium Feature");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error opening teacher dashboard: {ex.Message}", "Dashboard Error");
            }
        }

        private void ParallelReading_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var parallelWindow = new ParallelReadingWindow();
                parallelWindow.Show();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error opening parallel reading: {ex.Message}", "Parallel Reading Error");
            }
        }
    }
}
