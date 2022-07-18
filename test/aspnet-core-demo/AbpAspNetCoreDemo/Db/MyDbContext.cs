using System.Collections.Generic;
using Abp.EntityFrameworkCore;
using AbpAspNetCoreDemo.Core.Domain;
using Microsoft.EntityFrameworkCore;

namespace AbpAspNetCoreDemo.Db
{
    public class MyDbContext : AbpDbContext
    {
        public DbSet<Product> Products { get; set; }

        public DbSet<Customer> Customers { get; set; }

        public MyDbContext(DbContextOptions<MyDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Product>().HasData(new Product("Test product", 100)
            {
                Id = 1
            });
            
            modelBuilder.Entity<Customer>().HasData(new List<Customer>()
            {
                new Customer
                {
                    Id = 1,
                    TenantId = 1,
                    Name = "Volosoft",
                    Address = "123 Main street",
                },
                new Customer
                {
                    Id = 2,
                    TenantId = 1,
                    Name = "Microsoft",
                    Address = "456 Main street",
                }
            });
        }
    }
}