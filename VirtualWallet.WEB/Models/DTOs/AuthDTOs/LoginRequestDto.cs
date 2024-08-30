namespace VirtualWallet.WEB.Models.DTOs.AuthDTOs
{
    public class LoginRequestDto
    {
        public string UsernameOrEmail { get; set; }
        public string Password { get; set; }
    }
}
