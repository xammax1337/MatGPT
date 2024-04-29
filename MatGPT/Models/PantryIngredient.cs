using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MatGPT.Models
{
    public class PantryIngredient
    {
        [Key]
        public int PantryIngredientId { get; set; }

        [ForeignKey("Ingredient")]
        public int IngredientId { get; set; }
        public Ingredient Ingredient { get; set; }

        [ForeignKey("Pantry")]
        public int PantryId { get; set; }
        public Pantry Pantry { get; set; }
      
    }
}