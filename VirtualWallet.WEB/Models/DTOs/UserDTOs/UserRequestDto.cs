using VirtualWallet.DATA.Models.Enums;

namespace VirtualWallet.WEB.Models.DTOs.UserDTOs
{
    public class UserRequestDto
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
        public UserRole Role { get; set; }
    }

}
