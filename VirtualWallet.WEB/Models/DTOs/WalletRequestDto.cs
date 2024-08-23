using VirtualWallet.DATA.Models.Enums;

namespace VirtualWallet.WEB.Models.DTOs
{
    public class WalletRequestDto
    {
        public int Id { get; set; } 
        public int UserId { get; set; }
        public string Name { get; set; }
        public WalletType WalletType { get; set; }
        public CurrencyType Currency { get; set; }
    }
}
