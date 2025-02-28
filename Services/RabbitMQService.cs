using System.Text;
using System.Text.Json;
using RabbitMQ.Client;
using PaymentApi.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using PaymentApi.Models;
using RabbitMQ.Client.Exceptions;
using RabbitMQ.Client.Events;
using System.Text.Json.Serialization;


public class RabbitMqService : IRabbitMqService
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<RabbitMqService> _logger;

    public RabbitMqService(IConfiguration configuration, ILogger<RabbitMqService> logger)
    {
        _configuration = configuration;
        _logger = logger;
    }

    public async Task PublishTransactionAsync(Transaction transaction)
    {
        try
        {
            var factory = new ConnectionFactory()
            {
                HostName = _configuration["RabbitMQ:hostname"] ?? "localhost"
            };

            using var connection = await factory.CreateConnectionAsync();
            var channel = await connection.CreateChannelAsync();

            await channel.QueueDeclareAsync(queue: "payment_queue", durable: false, exclusive: false, autoDelete: false);

            var body = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(transaction));
            await channel.BasicPublishAsync(exchange: "", routingKey: "payment_queue", body: body);

            _logger.LogInformation($"Published transaction with ID: {transaction.TransactionId}");
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error publishing transaction: {ex.Message}");
        }
    }
}
