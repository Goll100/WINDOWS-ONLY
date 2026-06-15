using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
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
            try
            {
                if (!Directory.Exists(libraryFolder))
                    Directory.CreateDirectory(libraryFolder);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error initializing book service: {ex.Message}");
            }
        }

        public List<Book> GetAllBooks()
        {
            var books = new List<Book>();
            try
            {
                if (!Directory.Exists(libraryFolder))
                    return books;

                var files = Directory.GetFiles(libraryFolder, "*.*", SearchOption.TopDirectoryOnly)
                    .Where(f => f.EndsWith(".pdf", StringComparison.OrdinalIgnoreCase) || 
                                f.EndsWith(".epub", StringComparison.OrdinalIgnoreCase) || 
                                f.EndsWith(".scholastica", StringComparison.OrdinalIgnoreCase))
                    .ToList();
                    
                int id = 1;
                foreach (var file in files)
                {
                    try
                    {
                        books.Add(new Book
                        {
                            Id = id++,
                            Title = Path.GetFileNameWithoutExtension(file),
                            FilePath = file,
                            FileType = Path.GetExtension(file).TrimStart('.').ToLower()
                        });
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine($"Error adding book {file}: {ex.Message}");
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error getting all books: {ex.Message}");
            }
            return books;
        }

        public string GetBookContent(string filePath)
        {
            try
            {
                if (string.IsNullOrEmpty(filePath) || !File.Exists(filePath))
                    return "<html><body><p>File not found.</p></body></html>";

                string extension = Path.GetExtension(filePath).ToLower();

                if (extension == ".pdf")
                {
                    return GetPdfContent(filePath);
                }
                else if (extension == ".epub")
                {
                    return GetEpubContent(filePath);
                }
                else if (extension == ".scholastica")
                {
                    return GetScholasticaContent(filePath);
                }
                
                return "<html><body><p>Unsupported format: " + extension + "</p></body></html>";
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error getting book content: {ex.Message}");
                return $"<html><body><p>Error loading book: {ex.Message}</p></body></html>";
            }
        }

        private string GetPdfContent(string filePath)
        {
            try
            {
                using (var pdf = PdfDocument.Open(filePath))
                {
                    string html = "<html><head><style>body { font-family: Arial, sans-serif; }</style></head><body>";
                    foreach (var page in pdf.GetPages())
                    {
                        html += $"<div style='page-break-after:always; border: 1px solid #ccc; padding: 10px; margin: 10px 0;'>";
                        html += $"<h3>Page {page.Number}</h3>";
                        html += $"<p>{System.Net.WebUtility.HtmlEncode(page.Text)}</p>";
                        html += "</div>";
                    }
                    html += "</body></html>";
                    return html;
                }
            }
            catch (Exception ex)
            {
                return $"<html><body><p>Error reading PDF: {ex.Message}</p></body></html>";
            }
        }

        private string GetEpubContent(string filePath)
        {
            try
            {
                var book = EpubReader.ReadBook(filePath);
                string html = "<html><head><style>body { font-family: Georgia, serif; line-height: 1.6; }</style></head><body>";
                
                if (book.Content.Html != null)
                {
                    foreach (var item in book.Content.Html)
                    {
                        html += item.Value.Content;
                    }
                }
                
                html += "</body></html>";
                return html;
            }
            catch (Exception ex)
            {
                return $"<html><body><p>Error reading EPUB: {ex.Message}</p></body></html>";
            }
        }

        private string GetScholasticaContent(string filePath)
        {
            try
            {
                string content = File.ReadAllText(filePath, Encoding.UTF8);
                
                // If it's HTML, return as-is; otherwise, wrap in HTML tags
                if (content.StartsWith("<html", StringComparison.OrdinalIgnoreCase))
                {
                    return content;
                }
                
                return $"<html><head><style>body {{ font-family: Segoe UI, sans-serif; white-space: pre-wrap; }}</style></head><body>{System.Net.WebUtility.HtmlEncode(content)}</body></html>";
            }
            catch (Exception ex)
            {
                return $"<html><body><p>Error reading Scholastica file: {ex.Message}</p></body></html>";
            }
        }
    }
}
