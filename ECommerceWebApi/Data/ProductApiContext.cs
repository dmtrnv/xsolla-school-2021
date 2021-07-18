using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProductApi.Models;

namespace ProductApi.Data
{
    public class ProductApiContext : DbContext
    {
        public DbSet<Product> Products { get; set; }
        public DbSet<ProductManufacturer> Manufacturers { get; set; }
        public DbSet<ProductType> Types { get; set; }
        public DbSet<ProductSubtype> Subtypes { get; set; }

        public ProductApiContext(DbContextOptions<ProductApiContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Product>(ProductConfigure);
            modelBuilder.Entity<ProductManufacturer>(ManufacturerConfigure);
            modelBuilder.Entity<ProductType>(TypeConfigure);
            modelBuilder.Entity<ProductSubtype>(SubtypeConfigure);

            base.OnModelCreating(modelBuilder);
        }

        private void ProductConfigure(EntityTypeBuilder<Product> builder)
        {
            builder.HasKey(p => p.Id);

            builder.Property(p => p.Id)
                .ValueGeneratedOnAdd();

            builder.Property(p => p.Sku)
                .IsRequired();

            builder.Property(p => p.Name)
                .HasMaxLength(255)
                .IsRequired();

            builder.Property(p => p.ManufacturerId)
                .HasColumnName("Manufacturer_Id")
                .IsRequired();

            builder.Property(p => p.TypeId)
                .HasColumnName("Type_Id")
                .IsRequired();

            builder.Property(p => p.SubtypeId)
                .HasColumnName("Subtype_Id")
                .IsRequired();

            builder.Property(p => p.Cost)
                .HasColumnType("money")
                .IsRequired();
        }

        private void ManufacturerConfigure(EntityTypeBuilder<ProductManufacturer> builder)
        {
            builder.ToTable("Product_Manufacturer");

            builder.HasKey(m => m.Id);

            builder.Property(m => m.Id)
                .ValueGeneratedOnAdd();

            builder.Property(m => m.Name)
                .HasMaxLength(155)
                .IsRequired();

            builder.Property(m => m.Abbreviation)
                .HasMaxLength(7)
                .IsRequired();
        }

        private void TypeConfigure(EntityTypeBuilder<ProductType> builder)
        {
            builder.ToTable("Product_Type");

            builder.HasKey(t => t.Id);

            builder.Property(t => t.Id)
                .ValueGeneratedOnAdd();

            builder.Property(t => t.Name)
                .HasMaxLength(155)
                .IsRequired();

            builder.HasAlternateKey(t => t.Code);

            builder.Property(t => t.Code)
                .ValueGeneratedOnAdd();
        }

        private void SubtypeConfigure(EntityTypeBuilder<ProductSubtype> builder)
        {
            builder.ToTable("Product_Subtype");

            builder.HasKey(s => s.Id);

            builder.Property(s => s.Id)
                .ValueGeneratedOnAdd();

            builder.Property(s => s.Name)
                .HasMaxLength(155)
                .IsRequired();

            builder.HasAlternateKey(s => s.Code);

            builder.Property(s => s.Code)
                .ValueGeneratedOnAdd();
        }
    }
}
