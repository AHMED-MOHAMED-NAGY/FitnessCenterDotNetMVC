using Microsoft.EntityFrameworkCore;
using System.Drawing;
using Microsoft.Extensions.Configuration;

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
        public DbSet<Appointment> appointments { get; set; }

        private readonly IConfiguration _configuration;

        public FitnessContext(DbContextOptions<FitnessContext> options, IConfiguration configuration)
            : base(options)
        {
            _configuration = configuration;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer(_configuration.GetConnectionString("DefaultConnection"));
            }
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>()
                .HasOne(u => u.dailyGoal)
                .WithOne(d => d.User)
                .HasForeignKey<DailyGoal>(d => d.UserId);
            modelBuilder.Entity<Admin>().ToTable("Admins");
            modelBuilder.Entity<User>().ToTable("Users");
            modelBuilder.Entity<Cotch>().ToTable("Cotches");

            modelBuilder.Entity<Appointment>()
                .HasOne(a => a.User)
                .WithMany()
                .HasForeignKey(a => a.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Appointment>()
                .HasOne(a => a.Cotch)
                .WithMany()
                .HasForeignKey(a => a.CotchId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Exercise>()
                .Property(e => e.Price)
                .HasColumnType("decimal(18,2)");

            base.OnModelCreating(modelBuilder);
        }

    }
}
