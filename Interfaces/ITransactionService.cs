using PaymentApi.Models;
using PaymentApi.Models.Dtos;
namespace PaymentApi.Interfaces;

public interface ITransactionService
{
    Task<Transaction?> GetTransactionByIdAsync(Guid id);
    Task<IEnumerable<Transaction>> GetTransactionsAsync();
    
    Task<IEnumerable<TransactionHistory>> GetTransactionHistory();
    Task<Transaction> CreateTransactionAsync(Transaction transaction);
    Task<Transaction> UpdateTransactionAsync(Transaction transaction);
    Task DeleteTransactionAsync(Guid id);
    Task PublishTransactionAsync(Transaction transaction);
}
