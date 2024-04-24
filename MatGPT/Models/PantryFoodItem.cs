using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MatGPT.Models
{
    public class PantryFoodItem
    {
        [Key]
        public int PantryFoodItemId { get; set; }

        [ForeignKey("FoodItem")]
        public int FoodItemId { get; set; }
        public FoodItem FoodItem { get; set; }

        [ForeignKey("Pantry")]
        public int PantryId { get; set; }
        public Pantry Pantry { get; set; }
      
    }
}