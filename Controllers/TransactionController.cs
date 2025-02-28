using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using PaymentApi.Models;
using PaymentApi.Interfaces;
using PaymentApi.Models.Dtos;

namespace PaymentApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TransactionController : ControllerBase
    {
        private readonly ILogger<TransactionController> _logger;
        private readonly ITransactionService _transactionService;
        private readonly IRabbitMqService _rabbitMqService;

        public TransactionController(ITransactionService transactionService, IRabbitMqService rabbitMqService, ILogger<TransactionController> logger)
        {
            _transactionService = transactionService;
            _rabbitMqService = rabbitMqService;
            _logger = logger;
        }

        [HttpPost("add")]
        public async Task<IActionResult> AddTransaction([FromBody] Transaction transaction)
        {
            var newTransaction = await _transactionService.CreateTransactionAsync(transaction);
            _logger.LogInformation($"Added transaction with ID: {newTransaction.TransactionId}");
            return Created("", new { Id = newTransaction.TransactionId });
        }

        [HttpGet("{transactionID}")]
        public async Task<IActionResult> GetPaymentStatus(Guid transactionId)
        {
            var transaction = await _transactionService.GetTransactionByIdAsync(transactionId);
            if (transaction == null)
            {
                return NotFound();
            }
            return Ok(new { status = transaction.State });
        }

        [HttpGet]
        public async Task<IActionResult> GetTransactions() => Ok(await _transactionService.GetTransactionHistory());

        [HttpPost("begin/{transactionID}")]
        public async Task<IActionResult> BeginTransaction(Guid transactionId)
        {
            var transaction = await _transactionService.GetTransactionByIdAsync(transactionId);
            if (transaction == null)
            {
                return NotFound();
            }

            await _rabbitMqService.PublishTransactionAsync(transaction);
            return Ok();
        }
    }
}
