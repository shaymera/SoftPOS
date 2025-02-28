using PaymentApi.Context;
using PaymentApi.Models;
using RabbitMQ.Client;
using PaymentApi.Interfaces;
using Microsoft.EntityFrameworkCore;
using PaymentApi.Models.Dtos;

namespace PaymentApi.Repositories
{
    public class TransactionRepository(ApplicationDbContext context, IConnection connection) : ITransactionRepository
    {
        public async Task<Transaction> CreateTransaction(Transaction transaction)
        {
            await context.Transactions.AddAsync(transaction);
            await context.SaveChangesAsync();
            return transaction;
        }

        public async Task<Transaction?> GetTransactionById(Guid id) =>
            await context.Transactions.FindAsync(id);

        public async Task<IEnumerable<Transaction>> GetTransactions() =>
            await context.Transactions.ToListAsync();

        public async Task<IEnumerable<TransactionHistory>> GetTransactionHistory() => await context.Transactions
            .GroupBy(t => t.CreatedAt.Date)
            .OrderByDescending(g => g.Key)
            .Select(group => new TransactionHistory
            {
                DateString = group.Key.ToString("dd MMMM yyyy"),
                Transactions = group.OrderByDescending(t => t.CreatedAt)
            }).ToListAsync();

        public async Task<Transaction> UpdateTransaction(Transaction transaction)
        {
            context.Transactions.Update(transaction);
            await context.SaveChangesAsync();
            return transaction;
        }

        public async Task DeleteTransaction(Guid id)
        {
            var transaction = await GetTransactionById(id);
            if (transaction != null)
            {
                context.Transactions.Remove(transaction);
                await context.SaveChangesAsync();
            }
        }
    }
}
