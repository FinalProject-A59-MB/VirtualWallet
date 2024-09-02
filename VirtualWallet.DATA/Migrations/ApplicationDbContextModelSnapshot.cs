﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace VirtualWallet.DATA.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    partial class ApplicationDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "6.0.0")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder, 1L, 1);

            modelBuilder.Entity("VirtualWallet.DATA.Models.BlockedRecord", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<DateTime>("BlockedDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("Reason")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime?>("UnblockedDate")
                        .HasColumnType("datetime2");

                    b.Property<int>("UserId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("BlockedRecords");
                });

            modelBuilder.Entity("VirtualWallet.DATA.Models.Card", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<string>("CardHolderName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("CardNumber")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("CardType")
                        .HasColumnType("int");

                    b.Property<int>("Currency")
                        .HasColumnType("int");

                    b.Property<string>("Cvv")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("ExpirationDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("Issuer")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("PaymentProcessorToken")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("UserId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("Cards");
                });

            modelBuilder.Entity("VirtualWallet.DATA.Models.CardTransaction", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<decimal>("Amount")
                        .HasColumnType("decimal(18,2)");

                    b.Property<int>("CardId")
                        .HasColumnType("int");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime2");

                    b.Property<int>("Currency")
                        .HasColumnType("int");

                    b.Property<decimal?>("Fee")
                        .HasColumnType("decimal(18,2)");

                    b.Property<int>("Status")
                        .HasColumnType("int");

                    b.Property<int>("TransactionType")
                        .HasColumnType("int");

                    b.Property<int>("UserId")
                        .HasColumnType("int");

                    b.Property<int>("WalletId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("CardId");

                    b.HasIndex("UserId");

                    b.HasIndex("WalletId");

                    b.ToTable("CardTransactions");
                });

            modelBuilder.Entity("VirtualWallet.DATA.Models.RealCard", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<decimal>("Balance")
                        .HasColumnType("decimal(18,2)");

                    b.Property<string>("CardHolderName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("CardNumber")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("CardType")
                        .HasColumnType("int");

                    b.Property<int>("Currency")
                        .HasColumnType("int");

                    b.Property<string>("Cvv")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("ExpirationDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("Issuer")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("PaymentProcessorToken")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("RealCards");
                });

            modelBuilder.Entity("VirtualWallet.DATA.Models.RecurringPayment", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<decimal>("Amount")
                        .HasColumnType("decimal(18,2)");

                    b.Property<int>("Currency")
                        .HasColumnType("int");

                    b.Property<DateTime>("EndDate")
                        .HasColumnType("datetime2");

                    b.Property<int>("Frequency")
                        .HasColumnType("int");

                    b.Property<bool>("IsActive")
                        .HasColumnType("bit");

                    b.Property<DateTime>("NextPaymentDate")
                        .HasColumnType("datetime2");

                    b.Property<int>("RecipientId")
                        .HasColumnType("int");

                    b.Property<DateTime>("StartDate")
                        .HasColumnType("datetime2");

                    b.Property<int>("UserId")
                        .HasColumnType("int");

                    b.Property<int?>("WalletId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("RecipientId");

                    b.HasIndex("UserId");

                    b.HasIndex("WalletId");

                    b.ToTable("RecurringPayments");
                });

            modelBuilder.Entity("VirtualWallet.DATA.Models.User", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<int?>("BlockedRecordId")
                        .HasColumnType("int");

                    b.Property<DateTime?>("DeletedAt")
                        .HasColumnType("datetime2");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("FaceIdUrl")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("GoogleId")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int?>("MainWalletId")
                        .HasColumnType("int");

                    b.Property<string>("Password")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("PhotoIdUrl")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("Role")
                        .HasColumnType("int");

                    b.Property<string>("Username")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("VerificationStatus")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("BlockedRecordId");

                    b.HasIndex("MainWalletId")
                        .IsUnique()
                        .HasFilter("[MainWalletId] IS NOT NULL");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("VirtualWallet.DATA.Models.UserContact", b =>
                {
                    b.Property<int>("UserId")
                        .HasColumnType("int");

                    b.Property<int>("ContactId")
                        .HasColumnType("int");

                    b.Property<DateTime>("AddedDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("Description")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("SenderId")
                        .HasColumnType("int");

                    b.Property<int>("Status")
                        .HasColumnType("int");

                    b.HasKey("UserId", "ContactId");

                    b.HasIndex("ContactId");

                    b.HasIndex("SenderId");

                    b.ToTable("UserContacts");
                });

            modelBuilder.Entity("VirtualWallet.DATA.Models.UserProfile", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<string>("City")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Country")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime?>("DateOfBirth")
                        .HasColumnType("datetime2");

                    b.Property<string>("FirstName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("LastName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("PhoneNumber")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("PhotoUrl")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("PostalCode")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("State")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Street")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("UserId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("UserId")
                        .IsUnique();

                    b.ToTable("UserProfiles");
                });

            modelBuilder.Entity("VirtualWallet.DATA.Models.UserWallet", b =>
                {
                    b.Property<int>("UserId")
                        .HasColumnType("int");

                    b.Property<int>("WalletId")
                        .HasColumnType("int");

                    b.Property<int>("Id")
                        .HasColumnType("int");

                    b.Property<DateTime>("JoinedDate")
                        .HasColumnType("datetime2");

                    b.Property<int>("Role")
                        .HasColumnType("int");

                    b.HasKey("UserId", "WalletId");

                    b.HasIndex("WalletId");

                    b.ToTable("UserWallets");
                });

            modelBuilder.Entity("VirtualWallet.DATA.Models.Wallet", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<decimal>("Balance")
                        .HasColumnType("decimal(18,2)");

                    b.Property<int>("Currency")
                        .HasColumnType("int");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<Guid>("PublicId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<int>("UserId")
                        .HasColumnType("int");

                    b.Property<int>("WalletType")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("Wallets");
                });

            modelBuilder.Entity("VirtualWallet.DATA.Models.WalletTransaction", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime2");

                    b.Property<int>("Currency")
                        .HasColumnType("int");

                    b.Property<decimal>("DepositedAmount")
                        .HasColumnType("decimal(18,2)");

                    b.Property<int>("RecipientId")
                        .HasColumnType("int");

                    b.Property<int>("SenderId")
                        .HasColumnType("int");

                    b.Property<int>("Status")
                        .HasColumnType("int");

                    b.Property<string>("VerificationCode")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<decimal>("WithdrownAmount")
                        .HasColumnType("decimal(18,2)");

                    b.HasKey("Id");

                    b.HasIndex("RecipientId");

                    b.HasIndex("SenderId");

                    b.ToTable("WalletTransactions");
                });

            modelBuilder.Entity("VirtualWallet.DATA.Models.BlockedRecord", b =>
                {
                    b.HasOne("VirtualWallet.DATA.Models.User", "User")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("User");
                });

            modelBuilder.Entity("VirtualWallet.DATA.Models.Card", b =>
                {
                    b.HasOne("VirtualWallet.DATA.Models.User", "User")
                        .WithMany("Cards")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("User");
                });

            modelBuilder.Entity("VirtualWallet.DATA.Models.CardTransaction", b =>
                {
                    b.HasOne("VirtualWallet.DATA.Models.Card", "Card")
                        .WithMany("CardTransactions")
                        .HasForeignKey("CardId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("VirtualWallet.DATA.Models.User", "User")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("VirtualWallet.DATA.Models.Wallet", "Wallet")
                        .WithMany("CardTransactions")
                        .HasForeignKey("WalletId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Card");

                    b.Navigation("User");

                    b.Navigation("Wallet");
                });

            modelBuilder.Entity("VirtualWallet.DATA.Models.RecurringPayment", b =>
                {
                    b.HasOne("VirtualWallet.DATA.Models.User", "Recipient")
                        .WithMany()
                        .HasForeignKey("RecipientId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("VirtualWallet.DATA.Models.User", "User")
                        .WithMany("RecurringPayments")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("VirtualWallet.DATA.Models.Wallet", "Wallet")
                        .WithMany()
                        .HasForeignKey("WalletId");

                    b.Navigation("Recipient");

                    b.Navigation("User");

                    b.Navigation("Wallet");
                });

            modelBuilder.Entity("VirtualWallet.DATA.Models.User", b =>
                {
                    b.HasOne("VirtualWallet.DATA.Models.BlockedRecord", "BlockedRecord")
                        .WithMany()
                        .HasForeignKey("BlockedRecordId")
                        .OnDelete(DeleteBehavior.Restrict);

                    b.HasOne("VirtualWallet.DATA.Models.Wallet", "MainWallet")
                        .WithOne("User")
                        .HasForeignKey("VirtualWallet.DATA.Models.User", "MainWalletId")
                        .OnDelete(DeleteBehavior.Restrict);

                    b.Navigation("BlockedRecord");

                    b.Navigation("MainWallet");
                });

            modelBuilder.Entity("VirtualWallet.DATA.Models.UserContact", b =>
                {
                    b.HasOne("VirtualWallet.DATA.Models.User", "Contact")
                        .WithMany()
                        .HasForeignKey("ContactId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("VirtualWallet.DATA.Models.User", "Sender")
                        .WithMany()
                        .HasForeignKey("SenderId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("VirtualWallet.DATA.Models.User", "User")
                        .WithMany("Contacts")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("Contact");

                    b.Navigation("Sender");

                    b.Navigation("User");
                });

            modelBuilder.Entity("VirtualWallet.DATA.Models.UserProfile", b =>
                {
                    b.HasOne("VirtualWallet.DATA.Models.User", "User")
                        .WithOne("UserProfile")
                        .HasForeignKey("VirtualWallet.DATA.Models.UserProfile", "UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("User");
                });

            modelBuilder.Entity("VirtualWallet.DATA.Models.UserWallet", b =>
                {
                    b.HasOne("VirtualWallet.DATA.Models.User", "User")
                        .WithMany("UserWallets")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("VirtualWallet.DATA.Models.Wallet", "Wallet")
                        .WithMany("UserWallets")
                        .HasForeignKey("WalletId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("User");

                    b.Navigation("Wallet");
                });

            modelBuilder.Entity("VirtualWallet.DATA.Models.Wallet", b =>
                {
                    b.HasOne("VirtualWallet.DATA.Models.User", null)
                        .WithMany("Wallets")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("VirtualWallet.DATA.Models.WalletTransaction", b =>
                {
                    b.HasOne("VirtualWallet.DATA.Models.Wallet", "Recipient")
                        .WithMany()
                        .HasForeignKey("RecipientId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("VirtualWallet.DATA.Models.Wallet", "Sender")
                        .WithMany("WalletTransactions")
                        .HasForeignKey("SenderId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("Recipient");

                    b.Navigation("Sender");
                });

            modelBuilder.Entity("VirtualWallet.DATA.Models.Card", b =>
                {
                    b.Navigation("CardTransactions");
                });

            modelBuilder.Entity("VirtualWallet.DATA.Models.User", b =>
                {
                    b.Navigation("Cards");

                    b.Navigation("Contacts");

                    b.Navigation("RecurringPayments");

                    b.Navigation("UserProfile")
                        .IsRequired();

                    b.Navigation("UserWallets");

                    b.Navigation("Wallets");
                });

            modelBuilder.Entity("VirtualWallet.DATA.Models.Wallet", b =>
                {
                    b.Navigation("CardTransactions");

                    b.Navigation("User")
                        .IsRequired();

                    b.Navigation("UserWallets");

                    b.Navigation("WalletTransactions");
                });
#pragma warning restore 612, 618
        }
    }
}
