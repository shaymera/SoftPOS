using System;
using System.ComponentModel.Design;
using System.ComponentModel.DataAnnotations;

namespace PaymentApi.Models
{
    public class Transaction
    {
        [Key]
        public Guid TransactionId { get; set; } = Guid.NewGuid();
        public DateTime CreatedAt { get; set; }
        public TransactionState State { get; set; } = TransactionState.Pending;
        public required decimal Amount { get; set; }
        public CardType CardBrand { get; set; }
        public string? CardNumber { get; set; }
    }

    public enum CardType
    {
        Visa = 0,
        Mastercard = 1
    }

    public enum TransactionState
    {
        Pending = 0,
        Accepted = 1,
        Rejected = 2
    }
}
