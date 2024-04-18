namespace MatGPT.Models
{
    public class User
    {
        public int UserId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }

        public ICollection<Recipe> Recipes { get; set; }
        public ICollection<FoodItem> FoodItems { get; set; }
        public ICollection<FoodPreference> FoodPreferences { get; set;}
        public ICollection<KitchenSupply> KitchenSupplies { get; set;}
        public Credential Credential { get; set; }
    }
}
