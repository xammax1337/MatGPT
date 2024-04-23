using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MatGPT.Models
{
    public class FoodItem
    {
        [Key]
        public int FoodItemId { get; set; }
        public string FoodItemName { get; set; }
        public double? Amount { get; set; }
        [ForeignKey("User")]
        public int UserId { get; set; }

        public User User { get; set; }

        public virtual ICollection<PantryFoodItem> PantryFoodItems { get; set; }
    }
}
