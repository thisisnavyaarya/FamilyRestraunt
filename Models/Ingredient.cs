using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace FamilyRestraunt.Models
{
    public class Ingredient
    {
        public int IngredientId { get; set; }
        public string? Name { get; set; }

        // Navigation property for many-to-many relationship with Product via ProductIngredients
        [ValidateNever]
        public ICollection<ProductIngredients>? ProductIngredients { get; set; }
    }
}