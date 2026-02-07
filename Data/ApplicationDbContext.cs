using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using FamilyRestraunt.Models;

namespace FamilyRestraunt.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUsers>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Ingredient> Ingredients { get; set; }
        public DbSet<ProductIngredients> ProductIngredients { get; set; }
        public DbSet<Orders> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            // Configure composite primary key for ProductIngredients
            builder.Entity<ProductIngredients>()
                .HasKey(pi => new { pi.ProductId, pi.IngredientId });
            // Configure relationships
            builder.Entity<ProductIngredients>()
                .HasOne(pi => pi.Product)
                .WithMany(p => p.ProductIngredients)
                .HasForeignKey(pi => pi.ProductId);
            builder.Entity<ProductIngredients>()
                .HasOne(pi => pi.Ingredient)
                .WithMany(i => i.ProductIngredients)
                .HasForeignKey(pi => pi.IngredientId);
            
            //Seed data
            builder.Entity<Category>().HasData(
                new Category { Id = 1, Name = "Appetizers" },
                new Category { Id = 2, Name = "Entree" },
                new Category { Id = 3, Name = "Sides" },
                new Category { Id = 4, Name = "Desserts" },
                new Category { Id = 5, Name = "Beverages" }
            );
            builder.Entity<Ingredient>().HasData(
                new Ingredient { IngredientId = 1, Name = "Chicken" },
                new Ingredient { IngredientId = 2, Name = "Beef" },
                new Ingredient { IngredientId = 3, Name = "Lettuce" },
                new Ingredient { IngredientId = 4, Name = "Tomato" },
                new Ingredient { IngredientId = 5, Name = "Cheese" },
                new Ingredient { IngredientId = 6, Name = "Onion" }
                );
            builder.Entity<Product>().HasData(
                new Product { ProductId = 1, Name = "Chicken Salad", Description = "Fresh chicken salad with lettuce and tomato", Price = 9.99m,Stock=100, CategoryId = 1 },
                new Product { ProductId = 2, Name = "Beef Burger", Description = "Juicy beef burger with cheese and onion", Price = 12.99m, Stock = 100, CategoryId = 2 },
                new Product { ProductId = 3, Name = "French Fries", Description = "Crispy golden french fries", Price = 4.99m, Stock = 100, CategoryId = 3 },
                new Product { ProductId = 4, Name = "Chocolate Cake", Description = "Decadent chocolate cake slice", Price = 6.99m, Stock = 100, CategoryId = 4 },
                new Product { ProductId = 5, Name = "Lemonade", Description = "Refreshing homemade lemonade", Price = 2.99m, Stock = 100, CategoryId = 5 }
                );
             builder.Entity<ProductIngredients>().HasData(
                new ProductIngredients { ProductId = 1, IngredientId = 1 },
                new ProductIngredients { ProductId = 1, IngredientId = 3 },
                new ProductIngredients { ProductId = 1, IngredientId = 4 },
                new ProductIngredients { ProductId = 2, IngredientId = 2 },
                new ProductIngredients { ProductId = 2, IngredientId = 5 },
                new ProductIngredients { ProductId = 2, IngredientId = 6 },
                new ProductIngredients { ProductId = 3, IngredientId = 6 },
                new ProductIngredients { ProductId = 4, IngredientId = 5 },
                new ProductIngredients { ProductId = 5, IngredientId = 4 },
                new ProductIngredients { ProductId = 5, IngredientId = 3 },
                new ProductIngredients { ProductId = 5, IngredientId = 6 }
                );
        }
    }
}

