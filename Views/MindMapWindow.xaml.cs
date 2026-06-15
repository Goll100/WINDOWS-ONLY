using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace ScholasticaReader.Views
{
    public partial class MindMapWindow : Window
    {
        public MindMapWindow()
        {
            InitializeComponent();
            DrawMindMap();
        }

        private void DrawMindMap()
        {
            var ellipse = new Ellipse { Width = 100, Height = 50, Fill = Brushes.LightBlue };
            MapCanvas.Children.Add(ellipse);
            Canvas.SetLeft(ellipse, 300);
            Canvas.SetTop(ellipse, 200);

            var child = new Ellipse { Width = 80, Height = 40, Fill = Brushes.LightGreen };
            MapCanvas.Children.Add(child);
            Canvas.SetLeft(child, 100);
            Canvas.SetTop(child, 300);

            var line = new Line { X1 = 350, Y1 = 225, X2 = 140, Y2 = 320, Stroke = Brushes.Black, StrokeThickness = 2 };
            MapCanvas.Children.Add(line);
        }
    }
}
