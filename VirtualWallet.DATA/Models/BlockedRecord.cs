namespace VirtualWallet.DATA.Models
{
    public class BlockedRecord
    {
        public int Id { get; set; }
        public DateTime BlockedDate { get; set; }
        public DateTime? UnblockedDate { get; set; }
        public string Reason { get; set; }
        public int UserId { get; set; }
        public User User { get; set; }
    }

}
