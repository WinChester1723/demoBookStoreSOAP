using Application.Common.Interfaces;
using Infrastructure.Data;
using Infrastructure.Models;
using Nest;

namespace Infrastructure.Services
{
    public class BookService : IBookService
    {
        private readonly IBookRepository _bookRepository;
        private readonly IElasticSearchService _elasticSearchService; // add IElasticClient

        public BookService(IBookRepository bookRepository, IElasticSearchService elasticSearchService)
        {
            _bookRepository = bookRepository;
            _elasticSearchService = elasticSearchService;
        }

        public async Task<string> AddBook(string title, string author, int year)
        {
            // Checking for an existing workbook in ElasticSearch
            var existingBook = await _elasticSearchService.GetBookByIdAsync(title.GetHashCode());
            if (existingBook != null)
            {
                return $"Book '{title}' by {author} already exists and won't be added again.";
            }

            var book = new Book
            {
                Title = title,
                Author = author,
                Year = year,
            };

            // Saving to SQLite database
            await _bookRepository.AddBookAsync(book);

            // Indexing in ElasticSearch
            try
            {
                await _elasticSearchService.IndexBookAsync(book);
            }
            catch (Exception ex)
            {
                return $"Book '{title}' added to SQLite, but indexing in ElasticSearch failed: {ex.Message}";
            }

            return $"Book '{title}' by {author} added successfully and indexed in ElasticSearch!";
        }

        // Adding a method to search for books
        public async Task<List<Book>> SearchBooks(string query)
        {
            var searchResults = await _elasticSearchService.SearchBooks(query);

            return searchResults;
        }

        // Method for deleting a book by ID
        public async Task<string> DeleteBookById(int id)
        {
            await _bookRepository.DeleteBookAsync(id);

            try
            {
                await _elasticSearchService.DeleteBookByIdAsync(id);
            }
            catch (Exception ex)
            {
                return $"Book deleted from SQLite but failed to delete from ElasticSearch: {ex.Message}";
            }

            return $"Book with ID {id} deleted successfully!";
        }

        // Method for updating a book by ID
        public async Task<string> UpdateBook(int id, string title, string author, int year)
        {
            var book = await _bookRepository.GetBookByIdAsync(id);
            if (book == null)
            {
                return $"Book with ID {id} not found.";
            }

            book.Title = title;
            book.Author = author;
            book.Year = year;

            await _bookRepository.UpdateBookAsync(book);

            try
            {
                await _elasticSearchService.UpdateBookAsync(book);
            }
            catch (Exception ex)
            {
                return $"Book updated in SQLite but failed to update in ElasticSearch: {ex.Message}";
            }

            return $"Book with ID {id} updated successfully!";
        }

        // The method for getting the book by ID
        public async Task<Book?> GetBookById(int id)
        {
            var book = await _elasticSearchService.GetBookByIdAsync(id);
            return book ?? await _bookRepository.GetBookByIdAsync(id);
        }
    }
}