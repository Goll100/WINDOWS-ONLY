using System.Windows;

namespace ScholasticaReader.Views
{
    public partial class ParallelReadingWindow : Window
    {
        public ParallelReadingWindow()
        {
            InitializeComponent();
            Loaded += async (s, e) => {
                await LeftWebView.EnsureCoreWebView2Async(null);
                await RightWebView.EnsureCoreWebView2Async(null);
                LeftWebView.CoreWebView2.NavigateToString("<html><body>Left book</body></html>");
                RightWebView.CoreWebView2.NavigateToString("<html><body>Right book</body></html>");
            };
        }
    }
}
