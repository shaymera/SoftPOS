namespace PaymentApi.Models.Dtos;

public class TransactionHistory
{
    public required string DateString { get; set; }
    public required IEnumerable<Transaction> Transactions { get; set; }
}