using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using ScholasticaReader.Models;

namespace ScholasticaReader.Services
{
    public class AnnotationService
    {
        private string annotationFile;

        public AnnotationService()
        {
            try
            {
                string appData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
                string appFolder = Path.Combine(appData, "ScholasticaReader");
                
                if (!Directory.Exists(appFolder))
                    Directory.CreateDirectory(appFolder);
                    
                annotationFile = Path.Combine(appFolder, "annotations.json");
                
                if (!File.Exists(annotationFile))
                    File.WriteAllText(annotationFile, "[]");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error initializing annotation service: {ex.Message}");
            }
        }

        public void AddAnnotation(Annotation annotation)
        {
            try
            {
                if (annotation == null)
                    return;

                var annotations = new List<Annotation>();
                
                if (File.Exists(annotationFile))
                {
                    string json = File.ReadAllText(annotationFile);
                    if (!string.IsNullOrEmpty(json))
                    {
                        annotations = JsonSerializer.Deserialize<List<Annotation>>(json) ?? new List<Annotation>();
                    }
                }

                annotations.Add(annotation);
                string updatedJson = JsonSerializer.Serialize(annotations, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(annotationFile, updatedJson);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error adding annotation: {ex.Message}");
            }
        }

        public List<Annotation> GetAnnotationsForBook(int bookId)
        {
            try
            {
                if (!File.Exists(annotationFile))
                    return new List<Annotation>();

                string json = File.ReadAllText(annotationFile);
                if (string.IsNullOrEmpty(json))
                    return new List<Annotation>();

                var allAnnotations = JsonSerializer.Deserialize<List<Annotation>>(json) ?? new List<Annotation>();
                var bookAnnotations = new List<Annotation>();

                foreach (var annotation in allAnnotations)
                {
                    if (annotation.BookId == bookId)
                        bookAnnotations.Add(annotation);
                }

                return bookAnnotations;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error getting annotations: {ex.Message}");
                return new List<Annotation>();
            }
        }
    }
}
