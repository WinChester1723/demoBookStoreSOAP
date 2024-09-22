
using Application.Common.Interfaces;
using Infrastructure.Models;
using Nest;

namespace Infrastructure.Services
{
    public class ElasticSearchService : IElasticSearchService
    {
        private readonly IElasticClient _elasticClient;

        public ElasticSearchService(IElasticClient elasticClient)
        {
            _elasticClient = elasticClient;
        }

        public async Task IndexBookAsync(Book book)
        {
            var response = await _elasticClient.IndexDocumentAsync(book);
            if (!response.IsValid)
            {
                throw new InvalidOperationException($"Failed to index book in ElasticSearch: {response.OriginalException?.Message}");
            }
        }

        public async Task<Book?> GetBookByIdAsync(int id)
        {
            var response = await _elasticClient.GetAsync<Book>(id);
            if (!response.IsValid)
            {
                return null;
            }

            return response.Source;
        }

        public async Task DeleteBookByIdAsync(int id)
        {
            var response = await _elasticClient.DeleteAsync<Book>(id);
            if (!response.IsValid)
            {
                throw new InvalidOperationException($"Failed to delete book with ID {id} in ElasticSearch: {response.OriginalException?.Message}");
            }
        }

        public async Task UpdateBookAsync(Book book)
        {
            var response = await _elasticClient.UpdateAsync<Book>(book.Id, u => u.Doc(book));
            if (!response.IsValid)
            {
                throw new InvalidOperationException($"Failed to update book in ElasticSearch: {response.OriginalException?.Message}");
            }
        }

        public async Task<List<Book>> SearchBooks(string query)
        {
            var searchResponse = await _elasticClient.SearchAsync<Book>(s => s
                .Query(q => q
                    .MultiMatch(m => m
                        .Fields(f => f
                            .Field(ff => ff.Title)
                            .Field(ff => ff.Author)
                            .Field(ff => ff.Year))
                        .Query(query)
                    )
                )
            );

            if (!searchResponse.IsValid)
            {
                throw new InvalidOperationException($"Error during search operation: {searchResponse.ServerError?.Error.Reason}");
            }

            return searchResponse.Documents.ToList();
        }
    }
}