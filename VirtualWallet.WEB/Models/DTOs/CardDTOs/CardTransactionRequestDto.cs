using VirtualWallet.DATA.Models.Enums;

namespace VirtualWallet.WEB.Models.DTOs.CardDTOs
{
    public class CardTransactionRequestDto
    {
        public int CardId { get; set; }
        public int WalletId { get; set; }
        public decimal Amount { get; set; }
        public TransactionType TransactionType { get; set; }
    }

}
