using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MatGPT.Models
{
    public class FoodPreference
    {
        [Key]
        public int FoodPreferenceId { get; set; }
        public string FoodPreferenceName { get; set; }
        [ForeignKey("User")]
        public int UserId { get; set; }
        public User User { get; set; }


    }
}
