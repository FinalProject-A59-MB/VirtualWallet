using Microsoft.EntityFrameworkCore;
using VirtualWallet.DATA.Models;

namespace VirtualWallet.DATA.Context
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<UserProfile> UserProfiles { get; set; }
        public DbSet<UserContact> UserContacts { get; set; }
        public DbSet<Card> Cards { get; set; }
        public DbSet<WalletTransaction> WalletTransactions { get; set; }
        public DbSet<CardTransaction> CardTransactions { get; set; }
        public DbSet<RecurringPayment> RecurringPayments { get; set; }
        public DbSet<Wallet> Wallets { get; set; }
        public DbSet<UserWallet> UserWallets { get; set; }
        public DbSet<BlockedRecord> BlockedRecords { get; set; }
        public DbSet<RealCard> RealCards { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // One-to-One Relationships
            modelBuilder.Entity<User>()
                .HasOne(u => u.UserProfile)
                .WithOne(p => p.User)
                .HasForeignKey<UserProfile>(p => p.UserId);

            modelBuilder.Entity<User>()
                .HasOne(u => u.BlockedRecord)
                .WithOne(b => b.User)
                .HasForeignKey<BlockedRecord>(b => b.UserId);

            // Many-to-Many Relationship between User and Wallet through UserWallet
            modelBuilder.Entity<UserWallet>()
                .HasKey(uw => new { uw.UserId, uw.WalletId });

            modelBuilder.Entity<UserWallet>()
                .HasOne(uw => uw.User)
                .WithMany(u => u.UserWallets)
                .HasForeignKey(uw => uw.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<UserWallet>()
                .HasOne(uw => uw.Wallet)
                .WithMany(w => w.UserWallets)
                .HasForeignKey(uw => uw.WalletId)
                .OnDelete(DeleteBehavior.Restrict);

            // Many-to-Many Relationship for User Contacts
            modelBuilder.Entity<UserContact>()
                .HasKey(uc => new { uc.UserId, uc.ContactId });

            modelBuilder.Entity<UserContact>()
                .HasOne(uc => uc.User)
                .WithMany(u => u.Contacts)
                .HasForeignKey(uc => uc.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<UserContact>()
                .HasOne(uc => uc.Contact)
                .WithMany()
                .HasForeignKey(uc => uc.ContactId)
                .OnDelete(DeleteBehavior.Restrict);

            // One-to-Many Relationships
            modelBuilder.Entity<User>()
                .HasMany(u => u.Cards)
                .WithOne(c => c.User)
                .HasForeignKey(c => c.UserId);

            modelBuilder.Entity<User>()
                .HasMany(u => u.Wallets)
                .WithOne(w => w.Owner)
                .HasForeignKey(w => w.OwnerId);

            modelBuilder.Entity<WalletTransaction>()
                .HasOne(wt => wt.Sender)
                .WithMany()
                .HasForeignKey(wt => wt.SenderId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<WalletTransaction>()
                .HasOne(wt => wt.Recipient)
                .WithMany()
                .HasForeignKey(wt => wt.RecipientId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<CardTransaction>()
                .HasOne(ct => ct.Wallet)
                .WithMany(w => w.CardTransactions)
                .HasForeignKey(ct => ct.WalletId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<CardTransaction>()
                .HasOne(ct => ct.Card)
                .WithMany(c => c.CardTransactions)
                .HasForeignKey(ct => ct.CardId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<RecurringPayment>()
                .HasOne(rp => rp.User)
                .WithMany(u => u.RecurringPayments)
                .HasForeignKey(rp => rp.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<RecurringPayment>()
                .HasOne(rp => rp.Recipient)
                .WithMany()
                .HasForeignKey(rp => rp.RecipientId)
                .OnDelete(DeleteBehavior.Restrict);

            // Decimal precision
            modelBuilder.Entity<WalletTransaction>()
                .Property(wt => wt.Amount)
                .HasColumnType("decimal(18,2)");

            modelBuilder.Entity<CardTransaction>()
                .Property(ct => ct.Amount)
                .HasColumnType("decimal(18,2)");

            modelBuilder.Entity<RecurringPayment>()
                .Property(rp => rp.Amount)
                .HasColumnType("decimal(18,2)");

            modelBuilder.Entity<Wallet>()
                .Property(w => w.Balance)
                .HasColumnType("decimal(18,2)");

            modelBuilder.Entity<RealCard>()
                .Property(rc => rc.Balance)
                .HasColumnType("decimal(18,2)");

            base.OnModelCreating(modelBuilder);
        }
    }
}
