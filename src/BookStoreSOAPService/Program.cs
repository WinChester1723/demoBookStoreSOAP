using Application.Common.Interfaces;
using Infrastructure.Data;
using Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using SoapCore;

var builder = WebApplication.CreateBuilder(args);

// Add SOAP Service
builder.Services.AddSoapCore();
builder.Services.AddScoped<IBookService, BookService>();

// Add SQLite Context
builder.Services.AddDbContext<BookContext>(options => 
options.UseSqlite("Data Source=books.db"));

builder.Services.AddMvc().AddControllersAsServices();

var app = builder.Build();

app.UseRouting();

app.UseSoapEndpoint<IBookService>("/BookService.asmx", new SoapEncoderOptions());

app.Run();
