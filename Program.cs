using Microsoft.EntityFrameworkCore;
using PaymentApi.Context;
using Microsoft.OpenApi.Models;
using PaymentApi.Services;
using PaymentApi.Repositories;
using PaymentApi.Interfaces;
using RabbitMQ.Client;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend",
        builder => builder.WithOrigins("http://localhost:4200") // Replace with your frontend URL and port
                          .AllowAnyMethod()
                          .AllowAnyHeader()
                          .AllowCredentials()); // Allow credentials (cookies) to be sent
});

builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseNpgsql(@"Server=127.0.0.1; Port=5432; Database=Soft; User Id = username; Password = password;"));

builder.Services.AddSingleton<IConnection>(sp =>
{
    var config = sp.GetRequiredService<IConfiguration>();
    var factory = new ConnectionFactory()
    {
        HostName = config["RabbitMQ:hostname"] ?? "localhost"
    };
    return factory.CreateConnectionAsync().GetAwaiter().GetResult();
});

builder.Services.AddScoped<ITransactionRepository, TransactionRepository>();
builder.Services.AddScoped<ITransactionService, TransactionService>();
builder.Services.AddScoped<IRabbitMqService, RabbitMqService>();
builder.Services.AddHostedService<PaymentProcesserService>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.MapOpenApi();
    app.UseCors("AllowFrontend");
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
