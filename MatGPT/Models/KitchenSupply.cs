using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MatGPT.Models
{
    public class KitchenSupply
    {
        [Key]
        public int KitchenSupplyId { get; set; }
        public string KitchenSupplyName { get; set; }
        [ForeignKey("User")]
        public int? UserId { get; set; }
        public User User { get; set; }


    }
}
