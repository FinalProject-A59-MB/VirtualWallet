namespace VirtualWallet.WEB.Models.DTOs.UserDTOs
{
    public class UnblockRecordRequestDto
    {
        public int UserId { get; set; }
        public string Username { get; set; }
        public string Reason { get; set; }
    }
}
