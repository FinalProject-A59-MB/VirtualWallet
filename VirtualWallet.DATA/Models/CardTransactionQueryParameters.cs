namespace VirtualWallet.DATA.Models
{
    public class CardTransactionQueryParameters
    {
        public string? CardNumber { get; set; }
        public string? Wallet { get; set; }
        public int CardId { get; set; }
        public decimal Amount {  get; set; }
        public DateTime? CreatedAfter { get; set; }
        public DateTime? CreatedBefore { get; set; }
        public string SortBy { get; set; } = "CreatedAt";
        public string SortOrder { get; set; } = "desc";
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public string? TransactionType { get; set; }
    }

}
