using Microsoft.EntityFrameworkCore;
using System.Drawing;

namespace fitnessCenter.Models
{
    public class FitnessContext : DbContext
    {
        public DbSet<DailyGoal> dailyGoals { get; set; }
        public DbSet<Notification> notifications { get; set; }
        public DbSet<Man> men { get; set; }
        public DbSet<Admin> admins { get; set; }
        public DbSet<Cotch> cotches { get; set; }
        public DbSet<Exercise> exercises { get; set; }
        public DbSet<User> users { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseNpgsql("Host=localhost; DataBase=fitnessCenter; Username=postgres; Password=1234");
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //modelBuilder.Entity<User>()
            //    .HasOne(u => u.dailyGoal)
            //    .WithOne(d => d.User)
            //    .HasForeignKey<DailyGoal>(d => d.UserId);
            modelBuilder.Entity<Admin>().ToTable("Admins");
            modelBuilder.Entity<User>().ToTable("Users");
            modelBuilder.Entity<Cotch>().ToTable("Cotches");
            base.OnModelCreating(modelBuilder);
        }

    }
}
