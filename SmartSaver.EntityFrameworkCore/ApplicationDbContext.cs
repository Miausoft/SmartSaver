using Microsoft.EntityFrameworkCore;
using SmartSaver.EntityFrameworkCore.Models;

namespace SmartSaver.EntityFrameworkCore
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext() { }
        public ApplicationDbContext(DbContextOptions options) : base(options) { }

        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<Account> Accounts { get; set; }
        public virtual DbSet<Transaction> Transactions { get; set; }
        public virtual DbSet<Category> Categories { get; set; }
        public virtual DbSet<ProblemSuggestion> ProblemSuggestions { get; set; }
        public virtual DbSet<SolutionSuggestion> SolutionSuggestions { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>(u =>
            {
                u.HasIndex(u => u.Username).IsUnique();
                u.HasIndex(u => u.Email).IsUnique();
                u.HasIndex(u => u.Token).IsUnique();

                u.Property(u => u.Username).IsRequired();
                u.Property(u => u.Email).IsRequired();
                u.Property(u => u.Password).IsRequired(false);
                u.Property(u => u.Token).IsRequired(false);

                u.Property(u => u.DateJoined).HasDefaultValueSql("getdate()");
            });

            modelBuilder.Entity<Account>(a =>
            {
                a.HasKey(a => new { a.Id, a.UserId });
                a.HasIndex(a => new { a.UserId, a.Name }).IsUnique();

                a.Property(a => a.Goal).HasColumnType("decimal(18,2)");
                a.Property(a => a.Name).IsRequired();

                a.Property(a => a.GoalStartDate).HasDefaultValueSql("getdate()");
                a.Property(a => a.GoalEndDate).HasDefaultValueSql("dateadd(DD, 1 ,getdate())");

                a.HasCheckConstraint("PositiveGoal", "Goal > 0");
                a.HasCheckConstraint("StartEarlierThanEnd", "GoalStartDate < GoalEndDate");
            });

            modelBuilder.Entity<Transaction>(t =>
            {
                t.HasKey(t => new { t.Id, t.AccountId, t.UserId });

                t.Property(t => t.Amount).HasColumnType("decimal(18,2)");

                t.Property(t => t.CategoryId).HasDefaultValue(1);
                t.Property(t => t.ActionTime).HasDefaultValueSql("getdate()");
            });

            modelBuilder.Entity<Category>(c =>
            {
                c.HasIndex(c => c.Title).IsUnique();

                c.Property(c => c.TypeOfIncome).IsRequired();
                c.Property(c => c.Title).IsRequired();
            });

            modelBuilder.Entity<SolutionSuggestion>(s =>
            {
                s.HasIndex(s => s.SolutionText).IsUnique();
            });

            modelBuilder.Entity<ProblemSuggestion>(p =>
            {
                p.HasIndex(p => p.ProblemText).IsUnique();
            });
        }
    }
}
