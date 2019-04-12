using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace OnePay_Backend.Models
{
    public partial class OnePayContext : DbContext
    {
        public OnePayContext()
        {
        }

        public OnePayContext(DbContextOptions<OnePayContext> options)
            : base(options)
        {
        }

        public virtual DbSet<AccountType> AccountType { get; set; }
        public virtual DbSet<BproductType> BproductType { get; set; }
        public virtual DbSet<BusinessProduct> BusinessProduct { get; set; }
        public virtual DbSet<Message> Message { get; set; }
        public virtual DbSet<OnePayTransaction> OnePayTransaction { get; set; }
        public virtual DbSet<PaymentCycleType> PaymentCycleType { get; set; }
        public virtual DbSet<TransactionProviderType> TransactionProviderType { get; set; }
        public virtual DbSet<TransactionStatus> TransactionStatus { get; set; }
        public virtual DbSet<TransactionType> TransactionType { get; set; }
        public virtual DbSet<User> User { get; set; }
        public virtual DbSet<UserAccount> UserAccount { get; set; }
        public virtual DbSet<UserStatus> UserStatus { get; set; }
        public static string GetConnectionString()
        {
            return Startup.ConnectionString;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                var connection = GetConnectionString();
                optionsBuilder.UseSqlServer(connection, options => options.EnableRetryOnFailure());
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<AccountType>(entity =>
            {
                entity.Property(e => e.AccountTypeId).HasColumnName("AccountTypeID");

                entity.Property(e => e.AccountType1)
                    .IsRequired()
                    .HasColumnName("AccountType")
                    .HasMaxLength(10)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<BproductType>(entity =>
            {
                entity.ToTable("BProductType");

                entity.Property(e => e.BproductTypeId).HasColumnName("BProductTypeId");

                entity.Property(e => e.BproductType1)
                    .IsRequired()
                    .HasColumnName("BProductType")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.HasOne(d => d.AccountType)
                    .WithMany(p => p.BproductType)
                    .HasForeignKey(d => d.AccountTypeId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_BProductType_AccountType");
            });

            modelBuilder.Entity<BusinessProduct>(entity =>
            {
                entity.HasKey(e => e.PaccountId);

                entity.Property(e => e.PaccountId)
                    .HasColumnName("PAccountId")
                    .HasMaxLength(11)
                    .IsUnicode(false)
                    .ValueGeneratedNever();

                entity.Property(e => e.BproductName)
                    .IsRequired()
                    .HasColumnName("BProductName")
                    .HasMaxLength(20)
                    .IsUnicode(false);

                entity.Property(e => e.BproductTypeId).HasColumnName("BProductTypeId");

                entity.Property(e => e.UserId)
                    .IsRequired()
                    .HasColumnName("UserID")
                    .HasMaxLength(10)
                    .IsUnicode(false);

                entity.HasOne(d => d.BproductType)
                    .WithMany(p => p.BusinessProduct)
                    .HasForeignKey(d => d.BproductTypeId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_BusinessProduct_BProductType");

                entity.HasOne(d => d.PaymentCycleType)
                    .WithMany(p => p.BusinessProduct)
                    .HasForeignKey(d => d.PaymentCycleTypeId)
                    .HasConstraintName("FK_BusinessProduct_PaymentCycleType");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.BusinessProduct)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_BusinessProduct_User");
            });

            modelBuilder.Entity<Message>(entity =>
            {
                entity.Property(e => e.MessageBody)
                    .IsRequired()
                    .HasMaxLength(200)
                    .IsUnicode(false);

                entity.Property(e => e.Recipient)
                    .IsRequired()
                    .HasMaxLength(11)
                    .IsUnicode(false);

                entity.Property(e => e.Sender)
                    .IsRequired()
                    .HasMaxLength(20)
                    .IsUnicode(false);

                entity.Property(e => e.Status)
                    .IsRequired()
                    .HasMaxLength(20)
                    .IsUnicode(false);

                entity.Property(e => e.UserId)
                    .IsRequired()
                    .HasMaxLength(10)
                    .IsUnicode(false);

                entity.HasOne(d => d.User)
                    .WithMany(p => p.Message)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Message_User");
            });

            modelBuilder.Entity<OnePayTransaction>(entity =>
            {
                entity.HasKey(e => e.TransactionId);

                entity.Property(e => e.TransactionId)
                    .HasMaxLength(11)
                    .IsUnicode(false)
                    .ValueGeneratedNever();

                entity.Property(e => e.AccountId)
                    .IsRequired()
                    .HasMaxLength(11)
                    .IsUnicode(false);

                entity.Property(e => e.PaccountId)
                    .IsRequired()
                    .HasColumnName("PAccountId")
                    .HasMaxLength(11)
                    .IsUnicode(false);

                entity.Property(e => e.Timestamp)
                    .IsRequired()
                    .HasColumnName("timestamp")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.TransactionComment)
                    .IsRequired()
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.UserId)
                    .IsRequired()
                    .HasMaxLength(10)
                    .IsUnicode(false);

                entity.HasOne(d => d.Account)
                    .WithMany(p => p.OnePayTransaction)
                    .HasForeignKey(d => d.AccountId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_OnePayTransaction_UserAccount");

                entity.HasOne(d => d.Paccount)
                    .WithMany(p => p.OnePayTransaction)
                    .HasForeignKey(d => d.PaccountId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_OnePayTransaction_BusinessProduct");

                entity.HasOne(d => d.TransactionProvideType)
                    .WithMany(p => p.OnePayTransaction)
                    .HasForeignKey(d => d.TransactionProvideTypeId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_OnePayTransaction_TransactionProviderType");

                entity.HasOne(d => d.TransactionStatus)
                    .WithMany(p => p.OnePayTransaction)
                    .HasForeignKey(d => d.TransactionStatusId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_OnePayTransaction_TransactionStatus");

                entity.HasOne(d => d.TransactionType)
                    .WithMany(p => p.OnePayTransaction)
                    .HasForeignKey(d => d.TransactionTypeId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_OnePayTransaction_TransactionType");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.OnePayTransaction)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_OnePayTransaction_User");
            });

            modelBuilder.Entity<PaymentCycleType>(entity =>
            {
                entity.HasKey(e => e.PaymentCycleId);

                entity.Property(e => e.PaymentCycleType1)
                    .IsRequired()
                    .HasColumnName("PaymentCycleType")
                    .HasMaxLength(20)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<TransactionProviderType>(entity =>
            {
                entity.Property(e => e.TransactionPtype)
                    .IsRequired()
                    .HasColumnName("TransactionPType")
                    .HasMaxLength(50)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<TransactionStatus>(entity =>
            {
                entity.Property(e => e.TransactionType)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<TransactionType>(entity =>
            {
                entity.Property(e => e.TransactionTypeId).HasColumnName("TransactionTypeID");

                entity.Property(e => e.TransactionsType)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.Property(e => e.UserId)
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .ValueGeneratedNever();

                entity.Property(e => e.Password)
                    .IsRequired()
                    .HasColumnName("password")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.UserEmailAddress)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.UserName)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.UserPhoneNumber)
                    .IsRequired()
                    .HasMaxLength(12)
                    .IsUnicode(false);

                entity.HasOne(d => d.UserStatus)
                    .WithMany(p => p.User)
                    .HasForeignKey(d => d.UserStatusId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_User_UserStatus");
            });

            modelBuilder.Entity<UserAccount>(entity =>
            {
                entity.HasKey(e => e.AccountId);

                entity.Property(e => e.AccountId)
                    .HasColumnName("AccountID")
                    .HasMaxLength(11)
                    .IsUnicode(false)
                    .ValueGeneratedNever();

                entity.Property(e => e.AccountName)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.InterestRate).HasColumnType("decimal(2, 0)");

                entity.Property(e => e.Timestamp).IsRowVersion();

                entity.Property(e => e.UserId)
                    .HasMaxLength(10)
                    .IsUnicode(false);

                entity.HasOne(d => d.AccountType)
                    .WithMany(p => p.UserAccount)
                    .HasForeignKey(d => d.AccountTypeId)
                    .HasConstraintName("FK_UserAccount_AccountType");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.UserAccount)
                    .HasForeignKey(d => d.UserId)
                    .HasConstraintName("FK_UserAccount_User");
            });

            modelBuilder.Entity<UserStatus>(entity =>
            {
                entity.Property(e => e.UserStatusType)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);
            });
        }
    }
}
