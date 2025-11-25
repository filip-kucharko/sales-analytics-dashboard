using Microsoft.EntityFrameworkCore;
using SalesAnalytics.API.Models;

namespace SalesAnalytics.API.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options){}

        public DbSet<Sale> Sales { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Region> Regions { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Sale Configuration
            modelBuilder.Entity<Sale>(entity =>
            {
                // Primary Key
                entity.HasKey(e => e.SaleId);

                // Indexes für Performance
                entity.HasIndex(e => e.InvoiceDate)
                    .HasDatabaseName("IX_Sales_InvoiceDate");
                
                entity.HasIndex(e => e.ProductId)
                    .HasDatabaseName("IX_Sales_ProductId");
                
                entity.HasIndex(e => e.RegionId)
                    .HasDatabaseName("IX_Sales_RegionId");
                
                entity.HasIndex(e => e.InvoiceNo)
                    .HasDatabaseName("IX_Sales_InvoiceNo");

                // Foreign Key Relationships
                entity.HasOne(s => s.Product)
                    .WithMany(p => p.Sales)
                    .HasForeignKey(s => s.ProductId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(s => s.Customer)
                    .WithMany(c => c.Sales)
                    .HasForeignKey(s => s.CustomerId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(s => s.Region)
                    .WithMany(r => r.Sales)
                    .HasForeignKey(s => s.RegionId)
                    .OnDelete(DeleteBehavior.Restrict);

                // Decimal precision
                entity.Property(e => e.UnitPrice)
                    .HasPrecision(10, 2);
                
                entity.Property(e => e.TotalAmount)
                    .HasPrecision(10, 2);
            });

            // Product Configuration
            modelBuilder.Entity<Product>(entity =>
            {
                entity.HasKey(e => e.ProductId);
                
                entity.HasIndex(e => e.StockCode)
                    .IsUnique()
                    .HasDatabaseName("IX_Products_StockCode");
            });

            // Customer Configuration
            modelBuilder.Entity<Customer>(entity =>
            {
                entity.HasKey(e => e.CustomerId);
                
                entity.HasIndex(e => e.CustomerCode)
                    .IsUnique()
                    .HasDatabaseName("IX_Customers_CustomerCode");
            });

            // Region Configuration
            modelBuilder.Entity<Region>(entity =>
            {
                entity.HasKey(e => e.RegionId);
                
                entity.HasIndex(e => e.Country)
                    .IsUnique()
                    .HasDatabaseName("IX_Regions_Country");
            });
        }
    }
}