using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core
{
    public class TAContext : DbContext
    {
        public DbSet<Cities> Cities { get; set; }
        public DbSet<Countries> Countries { get; set; }
        public DbSet<Addresses> Addresses { get; set; }
        public DbSet<Locations> Locations { get; set; }
        public DbSet<Roles> Roles { get; set; }
        public DbSet<Users> Users { get; set; }
        public DbSet<Clients> Clients { get; set; }
        public DbSet<Statuses> Statuses { get; set; }
        public DbSet<Orders> Orderes { get; set; }
        public DbSet<Tours> Tours { get; set; }
        public DbSet<PaimentMethods> PaimentMethods { get; set; }
        public DbSet<ContractStatuses> ContractStatuses { get; set; }
        public DbSet<Contracts> Contracts { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("Server=localhost,1433;Database=TourismAgencyDB6;User Id=sa;Password=Braaah123!;TrustServerCertificate=True;");
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Orders>()
                .HasOne(o => o.Clients)
                .WithMany(c => c.Orders)
                .HasForeignKey(o => o.ClientID)
                .OnDelete(DeleteBehavior.Restrict);  // Вимикаємо каскадне видалення

            modelBuilder.Entity<Orders>()
                .HasOne(o => o.Users)
                .WithMany(u => u.Orders)
                .HasForeignKey(o => o.UserID)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Orders>()
                .HasOne(o => o.Tours)
                .WithMany(t => t.Orders)
                .HasForeignKey(o => o.TourID)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Orders>()
                .HasOne(o => o.Statuses)
                .WithMany(s => s.Orders)
                .HasForeignKey(o => o.StatusID)
                .OnDelete(DeleteBehavior.Restrict);
        }


    }
}
