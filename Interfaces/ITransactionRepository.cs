using PaymentApi.Models;
using PaymentApi.Models.Dtos;

namespace PaymentApi.Interfaces;

public interface ITransactionRepository
{
    Task<Transaction> CreateTransaction(Transaction transaction);
    Task<Transaction?> GetTransactionById(Guid id);
    Task<IEnumerable<Transaction>> GetTransactions();

    Task<IEnumerable<TransactionHistory>> GetTransactionHistory();
    Task<Transaction> UpdateTransaction(Transaction transaction);
    Task DeleteTransaction(Guid id);
}
