using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Threading.Tasks;

namespace Application.Common.Interfaces
{
    [ServiceContract]
    public interface IBookService
    {
        [OperationContract]
        Task<string> AddBook(string title, string author, int year);   
    }
}