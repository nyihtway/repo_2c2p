using System;

namespace _2C2P.DEMO.Domain.Models
{
    public class Transaction
    {
        public Guid Id { get; set; }
        public string TransactionId { get; set; }
        public decimal Amount { get; set; }
        public string CurrencyCode { get; set; }
        public DateTime TransactionDate { get; set; }
        public string Status { get; set; }
    }
}
