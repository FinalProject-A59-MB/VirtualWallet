using System.ComponentModel.DataAnnotations;

namespace VirtualWallet.WEB.Models.DTOs.AuthDTOs
{
    public class RegisterRequestDto
    {
        [Required]
        public string Username { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }
        [Required]
        public string Password { get; set; }
        [Required]
        [Compare("Password")]
        public string ConfirmPassword { get; set; }
    }
}
