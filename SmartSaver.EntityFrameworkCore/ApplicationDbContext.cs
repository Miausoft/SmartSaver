using Microsoft.EntityFrameworkCore;
using SmartSaver.EntityFrameworkCore.Models;

namespace SmartSaver.EntityFrameworkCore
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext() {}
        public ApplicationDbContext(DbContextOptions options) : base(options) {}

        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<Account> Accounts { get; set; }
        public virtual DbSet<Transaction> Transactions { get; set; }
        public virtual DbSet<Category> Categories { get; set; }
        public virtual DbSet<EmailVerification> EmailVerifications { get; set; }
        public virtual DbSet<ProblemSuggestion> ProblemSuggestions { get; set; }
        public virtual DbSet<SolutionSuggestion> SolutionSuggestions { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>()
                .HasIndex(u => u.Username)
                .IsUnique();

            modelBuilder.Entity<User>()
                .HasIndex(u => u.Email)
                .IsUnique();

            modelBuilder.Entity<User>()
                .Property(u => u.Username)
                .IsRequired();

            modelBuilder.Entity<User>()
                .Property(u => u.Email)
                .IsRequired();

            modelBuilder.Entity<User>()
                .Property(u => u.Password)
                .IsRequired(false);

            modelBuilder.Entity<User>()
                .Property(u => u.DateJoined)
                .HasDefaultValueSql("getdate()");

            modelBuilder.Entity<Account>()
                .Property(a => a.Goal)
                .HasColumnType("decimal(18,4)");

            modelBuilder.Entity<Account>()
                .Property(a => a.Goal)
                .HasDefaultValue(0);

            modelBuilder.Entity<Transaction>()
                .Property(t => t.Amount)
                .HasColumnType("decimal(18,4)");

            modelBuilder.Entity<Transaction>()
                .Property(t => t.CategoryId)
                .HasDefaultValue(1);

            modelBuilder.Entity<Transaction>()
                .Property(t => t.ActionTime)
                .HasDefaultValueSql("getdate()");

            modelBuilder.Entity<Account>(a =>
                a.HasCheckConstraint("PositiveGoal", "Goal >= 0"));

            modelBuilder.Entity<Category>()
                .Property(c => c.TypeOfIncome)
                .IsRequired();

            modelBuilder.Entity<Category>()
                .Property(c => c.Title)
                .IsRequired();

            modelBuilder.Entity<Category>()
                .HasIndex(c => c.Title)
                .IsUnique();

            modelBuilder.Entity<EmailVerification>()
                .Property(e => e.Token)
                .IsRequired();

            modelBuilder.Entity<EmailVerification>()
                .HasIndex(u => u.Token)
                .IsUnique();

            modelBuilder.Entity<SolutionSuggestion>()
                .HasIndex(s => s.SolutionText)
                .IsUnique();

            modelBuilder.Entity<ProblemSuggestion>()
                .HasIndex(p => p.ProblemText)
                .IsUnique();
        }
    }
}
