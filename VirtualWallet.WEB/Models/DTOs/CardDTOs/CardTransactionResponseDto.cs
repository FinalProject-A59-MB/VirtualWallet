using Microsoft.AspNetCore.Mvc;
using VirtualWallet.DATA.Models.Enums;

namespace VirtualWallet.WEB.Models.DTOs.CardDTOs
{
    public class CardTransactionResponseDto
    {
        public int Id { get; set; }
        public int CardId { get; set; }
        public int WalletId { get; set; }
        public decimal Amount { get; set; }
        public decimal? Fee { get; set; }
        public string Currency { get; set; }
        public TransactionType TransactionType { get; set; }
        public string Status { get; set; }

        public DateTime CreatedAt {  get; set; }
    }

}
