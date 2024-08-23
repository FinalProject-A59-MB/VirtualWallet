namespace VirtualWallet.WEB.Models.DTOs
{
    public class UserResponseDto
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Role { get; set; }
        public string VerificationStatus { get; set; }
        public UserProfileDto UserProfile { get; set; } 

        public ICollection<CardResponseDto> Cards { get; set; }
        //public ICollection<WalletResponseDto> Wallets { get; set; }

        public ICollection<UserContactDto> Contacts { get; set; }


    }

}
