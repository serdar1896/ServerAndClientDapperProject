using CoreProject.Entities.Models;
using Microsoft.EntityFrameworkCore;


namespace CoreProject.DataLayer.DataContext
{
    public class DatabaseContext : DbContext
    {
        public DatabaseContext(DbContextOptions<DatabaseContext> option) : base(option)
        {
        }
        protected override void OnModelCreating(ModelBuilder builder)
        // n-n convention Fluent API
        {
            builder.Entity<Stocks>().ToTable("Stocks");
                     
            builder.Entity<Customers>(config =>
            {
                config.Property(u => u.Tc).HasMaxLength(11);
                config.Property(u => u.Phone).HasMaxLength(11);
                config.ToTable("Customers");
            });
            builder.Entity<ProductComment>(config =>
            {
                config.Property(u => u.Comment).HasMaxLength(255);
                config.Property(u => u.CustomerId).IsRequired();
                config.ToTable("ProductComments");
            });
      

            base.OnModelCreating(builder);
        }


        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
        }

        public DbSet<Categories> categories { get; set; }
        public DbSet<Products> products { get; set; }
        public DbSet<Brands> brands { get; set; }
        public DbSet<ProductImages> productImages { get; set; }
        public DbSet<Roles> roles { get; set; }
        public DbSet<Stocks> stocks { get; set; }
        public DbSet<Provinces> provinces { get; set; }
        public DbSet<Counties> counties { get; set; }
        public DbSet<Country> country { get; set; }
        public DbSet<Address> address { get; set; }
        public DbSet<OrderDetails> orderDetails { get; set; }
        public DbSet<Orders> orderMasters { get; set; }
        public DbSet<ProductComment> productComment { get; set; }
        public DbSet<Customers> customers { get; set; }
        public DbSet<Cargos> cargos { get; set; }
        public DbSet<ProductFeatures> productFeatures { get; set; }
        public DbSet<AdminUsers> adminUsers { get; set; }

        public override int SaveChanges()
        {
            return base.SaveChanges();
        }

    }

}