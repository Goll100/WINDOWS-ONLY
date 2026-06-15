using System.Collections.Generic;
using System.Linq;
using ScholasticaReader.Data;
using ScholasticaReader.Models;

namespace ScholasticaReader.Services
{
    public class AnnotationService
    {
        public void AddAnnotation(Annotation annotation)
        {
            using (var db = new AppDbContext())
            {
                db.Annotations.Add(annotation);
                db.SaveChanges();
            }
        }

        public List<Annotation> GetAnnotationsForBook(int bookId)
        {
            using (var db = new AppDbContext())
            {
                return db.Annotations.Where(a => a.BookId == bookId).OrderByDescending(a => a.CreatedAt).ToList();
            }
        }
    }
} 
