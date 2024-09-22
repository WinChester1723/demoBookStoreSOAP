
using Infrastructure.Models;

namespace Application.Common.Interfaces
{
    public interface IElasticSearchService
    {
        Task IndexBookAsync(Book book);
        Task<Book?> GetBookByIdAsync(int id);
        Task DeleteBookByIdAsync(int id);
        Task UpdateBookAsync(Book book);
        Task<List<Book>> SearchBooks(string query);
    }
}