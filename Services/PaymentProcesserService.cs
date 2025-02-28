using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Text;
using System.Text.Json;
using PaymentApi.Context;
using PaymentApi.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PaymentApi.Interfaces;

namespace PaymentApi.Services
{
    public class PaymentProcesserService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private IChannel _channel;
        private IConnection _connection;
        private readonly IServiceScopeFactory _scopeFactory;

        public PaymentProcesserService(IServiceProvider serviceProvider, IConfiguration configuration, IServiceScopeFactory scopeFactory)
        {
            _serviceProvider = serviceProvider;
            _scopeFactory = scopeFactory;
            var _configuration = configuration;

            var factory = new ConnectionFactory()
            {
                HostName = _configuration["RabbitMQ:hostname"] ?? "localhost"
            };

            _connection = factory.CreateConnectionAsync().GetAwaiter().GetResult();
            _channel = _connection.CreateChannelAsync().GetAwaiter().GetResult();
            _channel.QueueDeclareAsync(queue: "payment_queue", durable: false, exclusive: false, autoDelete: false).Wait();
        }

        protected override async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            var consumer = new AsyncEventingBasicConsumer(_channel);

            consumer.ReceivedAsync += async (model, ea) =>
            {
                using var scope = _scopeFactory.CreateScope();
                var transactionService = scope.ServiceProvider.GetRequiredService<ITransactionService>();

                var body = ea.Body;
                var message = Encoding.UTF8.GetString(body.ToArray());
                var transaction = JsonSerializer.Deserialize<PaymentApi.Models.Transaction>(message);
                if (transaction == null)
                {
                    return;
                }

                var dbPayment = await transactionService.GetTransactionByIdAsync(transaction.TransactionId);
                if (dbPayment == null)
                {
                    return;
                }

                dbPayment.State = new Random().Next(0, 2) == 0 ? TransactionState.Rejected : TransactionState.Accepted;
                await transactionService.UpdateTransactionAsync(dbPayment);
                await _channel.BasicAckAsync(ea.DeliveryTag, multiple: false);
            };

            await _channel.BasicConsumeAsync(queue: "payment_queue", autoAck: false, consumer: consumer);

            while (!cancellationToken.IsCancellationRequested)
            {
                await Task.Delay(1000, cancellationToken);
            }
        }
    }
}
