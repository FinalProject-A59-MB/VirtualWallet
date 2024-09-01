using VirtualWallet.DATA.Models.Enums;

namespace VirtualWallet.WEB.Models.DTOs.WalletDTOs
{
    public class WalletResponseDto
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string Name { get; set; }
        public WalletType WalletType { get; set; }
        public decimal Balance { get; set; }
        public CurrencyType Currency { get; set; }
    }
}
