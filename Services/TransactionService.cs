using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using PaymentApi.Models;
using PaymentApi.Models.Dtos;
using PaymentApi.Repositories;
using PaymentApi.Interfaces;

namespace PaymentApi.Services;

public class TransactionService : ITransactionService
{
    private readonly ITransactionRepository _transactionRepository;
    private readonly IRabbitMqService _rabbitMqService;
    private readonly ILogger<TransactionService> _logger;

    public TransactionService(ITransactionRepository transactionRepository, IRabbitMqService rabbitMqService, ILogger<TransactionService> logger)
    {
        _transactionRepository = transactionRepository;
        _rabbitMqService = rabbitMqService;
        _logger = logger;
    }

    public Task<Transaction?> GetTransactionByIdAsync(Guid id) =>
        _transactionRepository.GetTransactionById(id);

    public Task<IEnumerable<Transaction>> GetTransactionsAsync() =>
        _transactionRepository.GetTransactions();

    public async Task<IEnumerable<TransactionHistory>> GetTransactionHistory() => 
        await _transactionRepository.GetTransactionHistory();
    public async Task<Transaction> CreateTransactionAsync(Transaction transaction)
    {
        _logger.LogInformation($"Creating transaction with ID: {transaction.TransactionId}");
        return await _transactionRepository.CreateTransaction(transaction);
    }

    public async Task<Transaction> UpdateTransactionAsync(Transaction transaction)
    {
        _logger.LogInformation($"Updating transaction with ID: {transaction.TransactionId}");
        return await _transactionRepository.UpdateTransaction(transaction);
    }

    public async Task DeleteTransactionAsync(Guid id)
    {
        _logger.LogInformation($"Deleting transaction with ID: {id}");
        await _transactionRepository.DeleteTransaction(id);
    }

    public Task PublishTransactionAsync(Transaction transaction) =>
        _rabbitMqService.PublishTransactionAsync(transaction);
}
