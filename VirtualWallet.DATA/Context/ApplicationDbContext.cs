using Microsoft.EntityFrameworkCore;
using VirtualWallet.DATA.Models;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    public DbSet<User> Users { get; set; }
    public DbSet<UserProfile> UserProfiles { get; set; }
    public DbSet<Card> Cards { get; set; }
    public DbSet<BlockedRecord> BlockedRecords { get; set; }
    public DbSet<CardTransaction> CardTransactions { get; set; }
    public DbSet<Wallet> Wallets { get; set; }
    public DbSet<WalletTransaction> WalletTransactions { get; set; }
    public DbSet<RecurringPayment> RecurringPayments { get; set; }
    public DbSet<UserContact> UserContacts { get; set; }
    public DbSet<UserWallet> UserWallets { get; set; }
    public DbSet<RealCard> RealCards { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Global Query Filters
        modelBuilder.Entity<User>().HasQueryFilter(u => !u.DeletedAt.HasValue);
        modelBuilder.Entity<BlockedRecord>().HasQueryFilter(br => !br.User.DeletedAt.HasValue);
        modelBuilder.Entity<CardTransaction>().HasQueryFilter(ct => !ct.User.DeletedAt.HasValue);

        // One-to-One Relationships
        modelBuilder.Entity<User>()
            .HasOne(u => u.UserProfile)
            .WithOne(p => p.User)
            .HasForeignKey<UserProfile>(p => p.UserId);

        modelBuilder.Entity<User>()
            .HasOne(u => u.MainWallet)
            .WithOne(w => w.User)
            .HasForeignKey<User>(u => u.MainWalletId)
            .OnDelete(DeleteBehavior.Restrict);

        // One-to-Many Relationships
        modelBuilder.Entity<User>()
            .HasOne(u => u.BlockedRecord)
            .WithMany()
            .HasForeignKey(u => u.BlockedRecordId)
            .OnDelete(DeleteBehavior.Restrict)
            .IsRequired(false);

        modelBuilder.Entity<BlockedRecord>()
            .HasOne(br => br.User)
            .WithMany()
            .HasForeignKey(u => u.UserId)
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


        modelBuilder.Entity<WalletTransaction>()
            .HasOne(wt => wt.Sender)
            .WithMany(u => u.WalletTransactions)
            .HasForeignKey(wt => wt.SenderId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<CardTransaction>()
            .HasOne(ct => ct.User)
            .WithMany()
            .HasForeignKey(ct => ct.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Card>()
            .HasMany(c => c.CardTransactions)
            .WithOne(ct => ct.Card)
            .HasForeignKey(ct => ct.CardId)
            .OnDelete(DeleteBehavior.Restrict);

        // Many-to-Many Relationships
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

        // Precision and Scale for Decimal
        modelBuilder.Entity<CardTransaction>()
            .Property(ct => ct.Amount)
            .HasColumnType("decimal(18,2)");

        modelBuilder.Entity<RealCard>()
            .Property(rc => rc.Balance)
            .HasColumnType("decimal(18,2)");

        modelBuilder.Entity<RecurringPayment>()
            .Property(rp => rp.Amount)
            .HasColumnType("decimal(18,2)");

        modelBuilder.Entity<Wallet>()
            .Property(w => w.Balance)
            .HasColumnType("decimal(18,2)");

        modelBuilder.Entity<WalletTransaction>()
            .Property(wt => wt.DepositedAmount)
            .HasColumnType("decimal(18,2)");

        base.OnModelCreating(modelBuilder);
    }


}
