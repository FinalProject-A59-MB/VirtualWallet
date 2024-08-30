namespace VirtualWallet.WEB.Models.DTOs.UserDTOs
{
    public class ChangePasswordRequestDto
    {
        public int UserId { get; set; }
        public string CurrentPassword { get; set; }
        public string NewPassword { get; set; }
    }

}
