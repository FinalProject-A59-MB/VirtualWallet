using System.ComponentModel.DataAnnotations;

namespace VirtualWallet.WEB.Models.ViewModels
{
    public class VerificationViewModel
    {
        public int UserId { get; set; }

        [Required]
        public IFormFile PhotoId { get; set; }

        [Required]
        public IFormFile LicenseId { get; set; }
    }
}
