using Application.Common.Interfaces;
using Infrastructure.Data;
using Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Nest;
using SoapCore;

var builder = WebApplication.CreateBuilder(args);

// Add SOAP Service
builder.Services.AddSoapCore();
builder.Services.AddScoped<IBookService, BookService>();

// Register IBookRepository и IElasticSearchService
builder.Services.AddScoped<IBookRepository, BookRepository>();
builder.Services.AddScoped<IElasticSearchService, ElasticSearchService>();

// Add SQLite Context
builder.Services.AddDbContext<BookContext>(options =>
options.UseSqlite("Data Source=books.db"));

// Setting up a connection to ElasticSearch
var elasticSettings = new ConnectionSettings(new Uri("http://localhost:9200"))
        .DefaultIndex("books"); // my index name

var elasticClient = new ElasticClient(elasticSettings);
builder.Services.AddSingleton<IElasticClient>(elasticClient);

builder.Services.AddMvc().AddControllersAsServices();

var app = builder.Build();

app.UseRouting();

// Using the correct UseSoapEndpoint call
app.UseSoapEndpoint<IBookService>("/BookService.asmx", new SoapEncoderOptions());

app.Run();
