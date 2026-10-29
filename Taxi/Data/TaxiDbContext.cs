using Microsoft.EntityFrameworkCore;
using Taxi.Models;

namespace Taxi.Data;

public class TaxiDbContext : DbContext
{
    public TaxiDbContext(DbContextOptions<TaxiDbContext> options) : base(options) {  }
    public DbSet<Car> Cars => Set<Car>();
    public DbSet<Driver> Drivers => Set<Driver>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Car>(entity =>
        {
            entity.ToTable("Cars");
            entity.HasKey(c => c.Id);
            entity.Property(c => c.Brand).IsRequired().HasMaxLength(50);
            entity.Property(c => c.Model).IsRequired().HasMaxLength(50);
            entity.Property(c => c.EngineVolume).IsRequired();
            entity.Property(c => c.Year).IsRequired();
        });

        modelBuilder.Entity<Driver>(entity =>
        {
            entity.ToTable("Drivers", t => t.HasCheckConstraint("CK_Drivers_Rating", "\"Rating\" >= 0 AND \"Rating\" <= 5"));
            entity.HasKey(d => d.Id);
            entity.Property(d => d.FullName).IsRequired().HasMaxLength(100);
            entity.Property(d => d.Rating).IsRequired();
            entity.Property(d => d.IsBusy).IsRequired();
            entity.HasOne(d => d.Car).WithMany(c => c.Drivers).HasForeignKey(d => d.CarId).OnDelete(DeleteBehavior.Cascade);
        });
    }
}