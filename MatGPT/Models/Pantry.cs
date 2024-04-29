using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MatGPT.Models
{
    public class Pantry
    {
        [Key]
        public int PantryId { get; set; }
        public string PantryName { get; set; }
        [ForeignKey("User")]
        public int UserId { get; set; }
        public User User { get; set; }

        public virtual ICollection<PantryIngredient> PantryIngredients { get; set; }
    }
}