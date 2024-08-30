namespace VirtualWallet.WEB.Models.DTOs.UserDTOs
{
    public class ChangePasswordResponseDto
    {
        public int UserId { get; set; }
        public bool IsSuccess { get; set; }
        public string Message { get; set; }
    }

}
