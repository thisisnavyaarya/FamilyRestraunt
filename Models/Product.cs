using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
namespace FamilyRestraunt.Models
{
    public class Product
    {
        public Product()
        {
            ProductIngredients = new List<ProductIngredients>();
        }
        public int ProductId { get; set; }
        public string? Name{ get; set; }
        public string? Description { get; set; }
        public decimal Price { get; set; }
        public int Stock { get; set; }
        public int CategoryId { get; set; }

        [NotMapped]
        public IFormFile? ImageFile { get; set; } // For uploading image
        public string? ImageUrl { get; set; } = "https://via.placeholder.com/150"; // Path to the image file
        [ValidateNever]
        public Category? Category { get; set; } // A product belongs to one category
        [ValidateNever]
        public ICollection<OrderItem>? OrderItems { get; set; } //a product can be in many order items
        [ValidateNever]
        public ICollection<ProductIngredients>? ProductIngredients { get; set; } // A product can have many ingredients
    }
}