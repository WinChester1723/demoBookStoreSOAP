using Application.Common.Interfaces;
using Infrastructure.Data;
using Infrastructure.Models;

namespace Infrastructure.Services
{
    public class BookService : IBookService
    {
        private readonly BookContext _context;

        public BookService(BookContext context)
        {
            _context = context;
        }

        public async Task<string> AddBook(string title, string author, int year)
        {
            var book = new Book
            {
                Title = title,
                Author = author,
                Year = year,
            };

            _context.Books.Add(book);
            await _context.SaveChangesAsync();

            return $"Book '{title}' by {author} added successfully!";
        }
    }
}