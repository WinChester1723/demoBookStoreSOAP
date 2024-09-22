
using Application.Common.Interfaces;
using Infrastructure.Models;
using Infrastructure.Services;
using Moq;

namespace BookStoreSOAPTests.Services
{
    public class BookServiceTests
    {
        private readonly BookService _bookService;
        private readonly Mock<IBookRepository> _bookRepositoryMock;
        private readonly Mock<IElasticSearchService> _elasticSearchServiceMock;

        public BookServiceTests()
        {
            _bookRepositoryMock = new Mock<IBookRepository>();
            _elasticSearchServiceMock = new Mock<IElasticSearchService>();
            _bookService = new BookService(_bookRepositoryMock.Object, _elasticSearchServiceMock.Object);
        }

        [Fact]
        public async Task AddBook_ShouldReturnSuccessMessage_WhenBookIsAddedSuccessfully()
        {
            // Arrange
            var title = "Война и мир";
            var author = "Лев Толстой";
            var year = 1869;

            _elasticSearchServiceMock.Setup(x => x.IndexBookAsync(It.IsAny<Book>())).Returns(Task.CompletedTask);
            _bookRepositoryMock.Setup(x => x.AddBookAsync(It.IsAny<Book>())).Returns(Task.CompletedTask);

            // Act
            var result = await _bookService.AddBook(title, author, year);

            // Assert
            Assert.Equal($"Book '{title}' by {author} added successfully and indexed in ElasticSearch!", result);
        }

        [Fact]
        public async Task SearchBooks_ShouldReturnListOfBooks_WhenBooksMatchQuery()
        {
            // Arrange
            var query = "Толстой";
            var books = new List<Book>
    {
        new Book { Id = 1, Title = "Война и мир", Author = "Лев Толстой", Year = 1869 },
        new Book { Id = 2, Title = "Анна Каренина", Author = "Лев Толстой", Year = 1877 }
    };

            _elasticSearchServiceMock.Setup(x => x.SearchBooks(query)).ReturnsAsync(books);

            // Act
            var result = await _bookService.SearchBooks(query);

            // Assert
            Assert.Equal(books.Count, result.Count);
            Assert.Equal(books[0].Title, result[0].Title);
        }

        [Fact]
        public async Task UpdateBook_ShouldReturnSuccessMessage_WhenBookIsUpdated()
        {
            // Arrange
            var id = 1;
            var title = "Анна Каренина";
            var author = "Лев Толстой";
            var year = 1877;

            var existingBook = new Book { Id = id, Title = "Старое название", Author = "Старый автор", Year = 1800 };

            _bookRepositoryMock.Setup(x => x.GetBookByIdAsync(id)).ReturnsAsync(existingBook);
            _bookRepositoryMock.Setup(x => x.UpdateBookAsync(It.IsAny<Book>())).Returns(Task.CompletedTask);
            _elasticSearchServiceMock.Setup(x => x.UpdateBookAsync(It.IsAny<Book>())).Returns(Task.CompletedTask);

            // Act
            var result = await _bookService.UpdateBook(id, title, author, year);

            // Assert
            Assert.Equal($"Book with ID {id} updated successfully!", result);
        }

        [Fact]
        public async Task DeleteBookById_ShouldReturnSuccessMessage_WhenBookIsDeleted()
        {
            // Arrange
            var id = 1;

            _bookRepositoryMock.Setup(x => x.DeleteBookAsync(id)).Returns(Task.CompletedTask);
            _elasticSearchServiceMock.Setup(x => x.DeleteBookByIdAsync(id)).Returns(Task.CompletedTask);

            // Act
            var result = await _bookService.DeleteBookById(id);

            // Assert
            Assert.Equal($"Book with ID {id} deleted successfully!", result);
        }
    }
}