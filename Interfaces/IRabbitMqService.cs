using PaymentApi.Models;
namespace PaymentApi.Interfaces;
public interface IRabbitMqService
{
    Task PublishTransactionAsync(Transaction transaction);
}
