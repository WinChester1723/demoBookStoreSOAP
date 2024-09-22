
using Application.Common.Interfaces;
using Infrastructure.Data;
using Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Nest;

namespace BookStoreSOAPTests.IntegrationTests
{
    public class IntegrationTestBase : IDisposable
    {
        protected readonly IServiceProvider ServiceProvider;
        protected readonly BookContext DbContext;
        protected readonly IElasticClient ElasticClient;

        protected IntegrationTestBase()
        {
            var serviceCollection = new ServiceCollection();

            // Use InMemory database for testing
            serviceCollection.AddDbContext<BookContext>(options =>
                options.UseInMemoryDatabase("TestDb"));

            // Register ElasticSearch settings (point to your actual ElasticSearch instance for integration testing)
            var elasticSettings = new ConnectionSettings(new Uri("http://localhost:9200"))
                .DefaultIndex("books");
            var elasticClient = new ElasticClient(elasticSettings);
            serviceCollection.AddSingleton<IElasticClient>(elasticClient);

            // Register your services
            serviceCollection.AddScoped<IBookRepository, BookRepository>();
            serviceCollection.AddScoped<IElasticSearchService, ElasticSearchService>();
            serviceCollection.AddScoped<IBookService, BookService>();

            ServiceProvider = serviceCollection.BuildServiceProvider();

            // Initialize DB context and ElasticClient
            DbContext = ServiceProvider.GetRequiredService<BookContext>();
            ElasticClient = elasticClient;
        }

        public void Dispose()
        {
            DbContext?.Dispose();
        }
    }
}