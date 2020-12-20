using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using Microsoft.EntityFrameworkCore;
using SmartSaver.EntityFrameworkCore.Models;

namespace SmartSaver.EntityFrameworkCore
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext() {}
        public ApplicationDbContext(DbContextOptions options) : base(options) {}

        public virtual DbSet<UserDto> Users { get; set; }
        public virtual DbSet<AccountDto> Accounts { get; set; }
        public virtual DbSet<TransactionDto> Transactions { get; set; }
        public virtual DbSet<CategoryDto> Categories { get; set; }
        public virtual DbSet<EmailVerificationDto> EmailVerifications { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<AccountDto>()
                .Property(a => a.Goal)
                .HasColumnType("decimal(18,4)");

            modelBuilder.Entity<TransactionDto>()
                .Property(t => t.Amount)
                .HasColumnType("decimal(18,4)");

            modelBuilder.Entity<TransactionDto>()
                .Property(t => t.CategoryId)
                .HasDefaultValue(1);

            modelBuilder.Entity<CategoryDto>()
                .Property(t => t.TypeOfIncome)
                .IsRequired();

            modelBuilder.Entity<UserDto>()
                .Property(t => t.Password)
                .IsRequired(false);

            modelBuilder.Entity<UserDto>()
                .Property(t => t.Username)
                .IsRequired(true);
        }
    }
}
