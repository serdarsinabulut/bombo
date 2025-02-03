using Microsoft.EntityFrameworkCore;
using projectbombo.Models;

namespace projectbombo.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<Debt> Debts { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Debt>()
                .HasOne(d => d.Lender)
                .WithMany(u => u.DebtsGiven)
                .HasForeignKey(d => d.LenderId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Debt>()
                .HasOne(d => d.Borrower)
                .WithMany(u => u.DebtsTaken)
                .HasForeignKey(d => d.BorrowerId)
                .OnDelete(DeleteBehavior.Restrict);
        }

    }
}
