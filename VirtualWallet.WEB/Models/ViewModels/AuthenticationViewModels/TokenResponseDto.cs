namespace VirtualWallet.WEB.Models.ViewModels.AuthenticationViewModels
{
    public class TokenResponseDto
    {
        /// <summary>
        /// The JWT token generated upon successful authentication or registration.
        /// </summary>
        public string Token { get; set; }
    }

}
