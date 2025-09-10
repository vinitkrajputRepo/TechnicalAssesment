using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }
    
    public DbSet<Product> Products { get; set; }
    public DbSet<Item> Items { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        modelBuilder.Entity<Product>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.ProductName).IsRequired().HasMaxLength(255);
            entity.Property(e => e.CreatedBy).IsRequired().HasMaxLength(100);
            entity.Property(e => e.CreatedOn).IsRequired();
            entity.Property(e => e.ModifiedBy).HasMaxLength(100);
            entity.Property(e => e.ModifiedOn);

            // Primary performance indexes for common queries
            entity.HasIndex(e => e.ProductName)
                .HasDatabaseName("IX_Product_ProductName");
            
            entity.HasIndex(e => e.CreatedBy)
                .HasDatabaseName("IX_Product_CreatedBy");
            
            entity.HasIndex(e => e.CreatedOn)
                .HasDatabaseName("IX_Product_CreatedOn");
            
            entity.HasIndex(e => e.ModifiedOn)
                .HasDatabaseName("IX_Product_ModifiedOn");

            // Composite indexes for optimized query patterns
            entity.HasIndex(e => new { e.CreatedBy, e.CreatedOn })
                .HasDatabaseName("IX_Product_CreatedBy_CreatedOn");
            
            entity.HasIndex(e => new { e.ProductName, e.CreatedOn })
                .HasDatabaseName("IX_Product_ProductName_CreatedOn");
            
            entity.HasIndex(e => new { e.ModifiedBy, e.ModifiedOn })
                .HasDatabaseName("IX_Product_ModifiedBy_ModifiedOn");

            // Covering index for search operations
            entity.HasIndex(e => new { e.ProductName, e.CreatedBy, e.CreatedOn })
                .HasDatabaseName("IX_Product_Search_Covering");
        });

        modelBuilder.Entity<Item>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.ProductId).IsRequired();
            entity.Property(e => e.Quantity).IsRequired();

            // Foreign key relationship
            entity.HasOne(e => e.Product)
                  .WithMany(p => p.Items)
                  .HasForeignKey(e => e.ProductId)
                  .OnDelete(DeleteBehavior.Cascade);

            // Performance indexes
            entity.HasIndex(e => e.ProductId)
                .HasDatabaseName("IX_Item_ProductId");
            
            entity.HasIndex(e => e.Quantity)
                .HasDatabaseName("IX_Item_Quantity");
            
            entity.HasIndex(e => new { e.ProductId, e.Quantity })
                .HasDatabaseName("IX_Item_ProductId_Quantity");
        });
    }
}
