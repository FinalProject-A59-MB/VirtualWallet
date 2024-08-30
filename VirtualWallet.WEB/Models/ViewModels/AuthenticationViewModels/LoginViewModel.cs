using System.ComponentModel.DataAnnotations;
using VirtualWallet.WEB.Attributes;

namespace VirtualWallet.WEB.Models.ViewModels.AuthenticationViewModels
{
    public class LoginViewModel
    {
        [Required]
        [CustomEmailOrUsername]
        public string UsernameOrEmail { get; set; }

        [Required(ErrorMessage = "Password is required.")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        public string? CustomError { get; set; }
    }

}
