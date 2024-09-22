using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Threading.Tasks;
using Infrastructure.Models;

namespace Application.Common.Interfaces
{
    [ServiceContract]
    public interface IBookService
    {
        [OperationContract]
        Task<string> AddBook(string title, string author, int year);

        [OperationContract]
        Task<List<Book>> SearchBooks(string query);

        [OperationContract]
        Task<string> DeleteBookById(int id);

        [OperationContract]
        Task<string> UpdateBook(int id, string title, string author, int year);

        [OperationContract]
        Task<Book> GetBookById(int id);
    }
}