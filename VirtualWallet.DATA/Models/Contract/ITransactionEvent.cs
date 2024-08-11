using VirtualWallet.DATA.Models.Enums;

namespace VirtualWallet.DATA.Models.Contract
{
    public interface ITransactionEvent
    {
        public int Id { get; set; }
        public decimal Amount { get; set; }
        public DateTime CreatedAt { get; set; }
        public CurrencyType Currency { get; set; }
        public TransactionStatus Status { get; set; }
        public string Origin { get; set; }

        public string Destination { get; set; }
    }
}