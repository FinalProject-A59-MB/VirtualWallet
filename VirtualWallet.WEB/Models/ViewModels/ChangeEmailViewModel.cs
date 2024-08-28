using System.ComponentModel.DataAnnotations;

namespace VirtualWallet.WEB.Models.ViewModels
{
    public class ChangeEmailViewModel
    {
        public int UserId { get; set; }

        [Required(ErrorMessage = "Current email is required.")]
        [EmailAddress(ErrorMessage = "Invalid email format.")]
        public string CurrentEmail { get; set; }

        [Required(ErrorMessage = "New email is required.")]
        [EmailAddress(ErrorMessage = "Invalid email format.")]
        public string NewEmail { get; set; }

        [Required(ErrorMessage = "Current password is required.")]
        [DataType(DataType.Password)]
        public string CurrentPassword { get; set; }
    }
}
