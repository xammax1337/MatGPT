using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MatGPT.Models
{
    public class Recipe
    {
        [Key]
        public int RecipeId { get; set; }
        public string RecipeName { get; set; }
        public string RecipeDescription { get; set; }
        public int? CookingTime { get; set; }
        [ForeignKey("User")]
        public int UserId { get; set; }

        public User User { get; set; }


    }
}
