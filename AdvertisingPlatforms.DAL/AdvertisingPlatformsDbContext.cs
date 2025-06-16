
using AdvertisingPlatforms.DAL.Entities;
using Microsoft.EntityFrameworkCore;

namespace AdvertisingPlatforms.DAL
{
    public class AdvertisingPlatformsDbContext : DbContext
    {
        DbSet<AdvertisementDb> Advertisements { get; set; }
        DbSet<LocationDb> Locations { get; set; }
        DbSet<AdvertisingPlatformDb> AdvertisingPlatforms { get; set; }

        public AdvertisingPlatformsDbContext(DbContextOptions<AdvertisingPlatformsDbContext> options)
            : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Конфигурация иерархии локаций
            modelBuilder.Entity<LocationDb>()
                .HasOne(l => l.Parent)
                .WithMany(l => l.Children)
                .HasForeignKey(l => l.ParentId)
                .OnDelete(DeleteBehavior.Restrict);

            // Конфигурация платформ
            modelBuilder.Entity<AdvertisingPlatformDb>()
                .HasOne(p => p.Advertisement)
                .WithMany(a => a.AdvertisingPlatforms)
                .HasForeignKey(p => p.AdvertisementId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<AdvertisingPlatformDb>()
                .HasOne(p => p.Location)
                .WithMany(l => l.AdvertisingPlatforms)
                .HasForeignKey(p => p.LocationId)
                .OnDelete(DeleteBehavior.Restrict);

            // Индексы
            modelBuilder.Entity<LocationDb>()
                .HasIndex(l => l.Path)
                .IsUnique();

            modelBuilder.Entity<AdvertisementDb>()
                .HasIndex(a => a.Name)
                .IsUnique();
        }
    }
}