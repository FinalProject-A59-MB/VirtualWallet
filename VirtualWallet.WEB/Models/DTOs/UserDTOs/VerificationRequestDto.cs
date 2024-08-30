namespace VirtualWallet.WEB.Models.DTOs.UserDTOs
{
    public class VerificationRequestDto
    {
        public int UserId { get; set; }
        public IFormFile PhotoId { get; set; }
        public IFormFile LicenseId { get; set; }
    }

}
