using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace AstraReconsSaas.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<SubscriptionPlan> SubscriptionPlans { get; set; }
        public DbSet<UserSubscription> UserSubscriptions { get; set; }
        public DbSet<Payment> Payments { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Configure relationships
            builder.Entity<UserSubscription>()
                .HasOne(us => us.User)
                .WithMany(u => u.Subscriptions)
                .HasForeignKey(us => us.UserId);

            builder.Entity<UserSubscription>()
                .HasOne(us => us.SubscriptionPlan)
                .WithMany(sp => sp.Subscriptions)
                .HasForeignKey(us => us.SubscriptionPlanId);

            builder.Entity<Payment>()
                .HasOne(p => p.User)
                .WithMany()
                .HasForeignKey(p => p.UserId);

            builder.Entity<Payment>()
                .HasOne(p => p.UserSubscription)
                .WithMany()
                .HasForeignKey(p => p.UserSubscriptionId);

            // Seed initial subscription plans
            builder.Entity<SubscriptionPlan>().HasData(
                new SubscriptionPlan 
                { 
                    Id = 1,
                    Name = "Basic",
                    Description = "Basic plan for small teams",
                    MonthlyPrice = 29.99m,
                    YearlyPrice = 299.99m,
                    MaxUsers = 5,
                    Features = "{\"storage\":\"10GB\",\"projects\":\"10\",\"support\":\"Email\"}",
                    IsActive = true
                },
                new SubscriptionPlan 
                { 
                    Id = 2,
                    Name = "Pro",
                    Description = "Professional plan for growing businesses",
                    MonthlyPrice = 79.99m,
                    YearlyPrice = 799.99m,
                    MaxUsers = 20,
                    Features = "{\"storage\":\"50GB\",\"projects\":\"Unlimited\",\"support\":\"Priority Email\"}",
                    IsActive = true
                },
                new SubscriptionPlan 
                { 
                    Id = 3,
                    Name = "Enterprise",
                    Description = "Enterprise plan for large organizations",
                    MonthlyPrice = 199.99m,
                    YearlyPrice = 1999.99m,
                    MaxUsers = 100,
                    Features = "{\"storage\":\"200GB\",\"projects\":\"Unlimited\",\"support\":\"24/7 Phone & Email\"}",
                    IsActive = true
                }
            );
        }
    }

    public class IdentityDbContext<T>
    {
    }
}