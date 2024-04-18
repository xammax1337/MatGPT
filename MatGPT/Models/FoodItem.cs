namespace MatGPT.Models
{
    public class FoodItem
    {
        public int FoodItemId { get; set; }
        public string FoodItemName { get; set; }
        public double Amount { get; set;}
        public int UserId { get; set; }
        public User User { get; set; }


    }
}
