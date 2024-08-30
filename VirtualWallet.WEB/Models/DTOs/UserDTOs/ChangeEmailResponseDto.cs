namespace VirtualWallet.WEB.Models.DTOs.UserDTOs
{
    public class ChangeEmailResponseDto
    {
        public int UserId { get; set; }
        public bool IsSuccess { get; set; }
        public string Message { get; set; }
        public string NewEmail { get; set; }
    }

}
