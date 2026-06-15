using System;

namespace ScholasticaReader.Models
{
    public class Annotation
    {
        public int Id { get; set; }
        public int BookId { get; set; }
        public string Type { get; set; }
        public string Text { get; set; }
        public int Page { get; set; }
        public DateTime CreatedAt { get; set; }
    }
} 
