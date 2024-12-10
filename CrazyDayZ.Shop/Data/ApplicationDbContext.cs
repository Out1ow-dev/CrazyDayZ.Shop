using CrazyDayZ.Shop.Models;
using CrazyDayZ.Shop.Models.Config;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Emit;

namespace CrazyDayZ.Shop.Data
{
    public class ApplicationDbContext : IdentityDbContext<AppIdentityUser, IdentityRole, string>
    {
        public DbSet<ServerData> Servers { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Item> Items { get; set; }
        public DbSet<Car> Cars { get; set; }
        public DbSet<Attachment> Attachments { get; set; }
        public DbSet<Purchase> Purchases { get; set; }
        public DbSet<Lottery> Lotteries { get; set; }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Purchase>()
               .HasOne(p => p.User) 
               .WithMany(u => u.Purchase) 
               .HasForeignKey(p => p.UserId);

        }
    }

}
