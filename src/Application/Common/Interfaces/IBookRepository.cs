
using Infrastructure.Models;

namespace Application.Common.Interfaces
{
    public interface IBookRepository
    {
        Task<Book?> GetBookByIdAsync(int id);
        Task AddBookAsync(Book book);
        Task UpdateBookAsync(Book book);
        Task DeleteBookAsync(int id);
    }
}