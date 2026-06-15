5 using Microsoft.EntityFrameworkCore;
using ScholasticaReader.Models;
using System.IO;

namespace ScholasticaReader.Data
{
    public class AppDbContext : DbContext
    {
        public DbSet<Book> Books { get; set; }
        public DbSet<Annotation> Annotations { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            string dbPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "ScholasticaReader", "reader.db");
            options.UseSqlite($"Data Source={dbPath}");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Annotation>().Property(a => a.Id).ValueGeneratedOnAdd();
        }
    }
}
