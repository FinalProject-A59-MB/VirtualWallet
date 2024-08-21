using VirtualWallet.DATA.Models.Enums;
using VirtualWallet.DATA.Models;

namespace VirtualWallet.WEB.Models.DTOs
{
    public class WalletResponseDto
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public User User { get; set; }
        public string Name { get; set; }
        public WalletType WalletType { get; set; }
        public decimal Balance { get; set; }
        public CurrencyType Currency { get; set; }
    }
}
