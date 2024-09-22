
using System.Text;
using Application.Common.Interfaces;
using Infrastructure.Models;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace BookStoreSOAPTests.IntegrationTests
{
    public class BookServiceIntegrationTests : IntegrationTestBase
    {
        private readonly IBookService _bookService;

        public BookServiceIntegrationTests()
        {
            _bookService = ServiceProvider.GetRequiredService<IBookService>();
        }

        [Fact]
        public async Task AddBook_Should_AddToDatabase_And_IndexInElasticSearch()
        {
            // Arrange
            var title = "Тестовая книга";
            var author = "Тестовый автор";
            var year = 2021;

            // Act
            var result = await _bookService.AddBook(title, author, year);

            // Assert
            Assert.Equal($"Book '{title}' by {author} added successfully and indexed in ElasticSearch!", result);

            // Verify the book was added to the in-memory database
            var bookInDb = await DbContext.Books.FirstOrDefaultAsync(b => b.Title == title);
            Assert.NotNull(bookInDb);

            // Verify the book was indexed in ElasticSearch
            var searchResponse = await ElasticClient.SearchAsync<Book>(s => s
                .Query(q => q.Match(m => m.Field(f => f.Title).Query(title))));

            Assert.True(searchResponse.Documents.Count > 0);
        }

        [Fact]
        public async Task SearchBooks_Should_ReturnResultsFromElasticSearch()
        {
            // Arrange
            var title = "Тестовая книга";
            var author = "Тестовый автор";
            var year = 2021;

            await _bookService.AddBook(title, author, year);

            // Act
            var searchResults = await _bookService.SearchBooks("Тестовая книга");

            // Assert
            Assert.NotEmpty(searchResults);
            Assert.Contains(searchResults, book => book.Title == title && book.Author == author);
        }

        [Fact]
        public async Task UpdateBook_Should_UpdateDatabaseAndElasticSearch()
        {
            // Arrange
            var title = "Тестовая книга";
            var author = "Тестовый автор";
            var year = 2021;

            await _bookService.AddBook(title, author, year);
            var bookInDb = await DbContext.Books.FirstOrDefaultAsync(b => b.Title == title);

            // Act
            var updatedTitle = "Обновленная книга";
            var result = await _bookService.UpdateBook(bookInDb!.Id, updatedTitle, author, year);

            // Assert
            Assert.Equal($"Book with ID {bookInDb.Id} updated successfully!", result);

            // Verify the update in ElasticSearch
            var searchResponse = await ElasticClient.SearchAsync<Book>(s => s
                .Query(q => q.Match(m => m.Field(f => f.Title).Query(updatedTitle))));

            Assert.True(searchResponse.Documents.Count > 0);
        }

        [Fact]
        public async Task DeleteBookById_Should_RemoveFromDatabaseAndElasticSearch()
        {
            // Arrange
            var title = "Тестовая книга";
            var author = "Тестовый автор";
            var year = 2021;

            await _bookService.AddBook(title, author, year);
            var bookInDb = await DbContext.Books.FirstOrDefaultAsync(b => b.Title == title);

            // Act
            var result = await _bookService.DeleteBookById(bookInDb!.Id);

            // Assert
            Assert.Equal($"Book with ID {bookInDb.Id} deleted successfully!", result);

            // Verify the book was removed from the database
            var deletedBook = await DbContext.Books.FindAsync(bookInDb.Id);
            Assert.Null(deletedBook);

            // Verify the book was removed from ElasticSearch
            var searchResponse = await ElasticClient.GetAsync<Book>(bookInDb.Id);
            Assert.False(searchResponse.Found);
        }
    }
}