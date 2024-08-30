namespace VirtualWallet.WEB.Models.DTOs.UserDTOs
{
    public class ChangeEmailRequestDto
    {
        public int UserId { get; set; }
        public string CurrentPassword { get; set; }
        public string NewEmail { get; set; }
    }

}
