namespace VirtualWallet.DATA.Models
{
    public class UserProfile
    {
        public int Id { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;

        private string? _photoUrl;
        public string? PhotoUrl
        {
            get => _photoUrl;
            set => _photoUrl = value;
        }

        public string PhoneNumber { get; set; } = string.Empty;
        public DateTime? DateOfBirth { get; set; }
        public string Street { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public string State { get; set; } = string.Empty;
        public string PostalCode { get; set; } = string.Empty;
        public string Country { get; set; } = string.Empty;
        public int UserId { get; set; }
        public User User { get; set; }

        // Constructor to initialize the PhotoUrl
        public UserProfile()
        {
            _photoUrl = _photoUrl ?? "/images/default.jpg";
        }
    }

}
