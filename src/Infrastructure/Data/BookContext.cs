
using Infrastructure.Models;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Data
{
    public class BookContext : DbContext
    {
        public BookContext(DbContextOptions<BookContext> options) : base (options){}

        public DbSet<Book> Books { get; set; }

        protected override void  OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlite("Data Source=books.db");
            }
        }
    }
}