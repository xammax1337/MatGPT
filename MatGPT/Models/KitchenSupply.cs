namespace MatGPT.Models
{
    public class KitchenSupply
    {
        public int KitchenSupplyId { get; set; }
        public string KitchenSupplyName { get; set; }
        public int UserId { get; set; }
        public User User { get; set; }


    }
}
