using System.Collections.Generic;
using System.IO;
using System.Linq;
using ScholasticaReader.Models;
using VersOne.Epub;
using UglyToad.PdfPig;

namespace ScholasticaReader.Services
{
    public class BookService
    {
        private string libraryFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "ScholasticaLibrary");

        public BookService()
        {
            if (!Directory.Exists(libraryFolder))
                Directory.CreateDirectory(libraryFolder);
        }

        public List<Book> GetAllBooks()
        {
            var books = new List<Book>();
            var files = Directory.GetFiles(libraryFolder, "*.*", SearchOption.TopDirectoryOnly)
                .Where(f => f.EndsWith(".pdf") || f.EndsWith(".epub") || f.EndsWith(".scholastica"));
            int id = 1;
            foreach (var file in files)
            {
                books.Add(new Book
                {
                    Id = id++,
                    Title = Path.GetFileNameWithoutExtension(file),
                    FilePath = file,
                    FileType = Path.GetExtension(file).TrimStart('.')
                });
            }
            return books;
        }

        public string GetBookContent(string filePath)
        {
            if (filePath.EndsWith(".pdf"))
            {
                using (var pdf = PdfDocument.Open(filePath))
                {
                    string html = "<html><body>";
                    foreach (var page in pdf.GetPages())
                    {
                        html += $"<div style='page-break-after:always'><h3>Page {page.Number}</h3><p>{page.Text}</p></div>";
                    }
                    html += "</body></html>";
                    return html;
                }
            }
            else if (filePath.EndsWith(".epub"))
            {
                var book = EpubReader.ReadBook(filePath);
                string html = "<html><body>";
                foreach (var item in book.Content.Html)
                {
                    html += item.Value.Content;
                }
                html += "</body></html>";
                return html;
            }
            else if (filePath.EndsWith(".scholastica"))
            {
                return File.ReadAllText(filePath);
            }
            return "<html><body>Unsupported format</body></html>";
        }
    }
} 
