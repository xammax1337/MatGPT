namespace MatGPT.Models
{
    public class User
    {
        public int UserId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }

        public virtual ICollection<Recipe> Recipes { get; set; }
        public virtual ICollection<FoodItem> FoodItems { get; set; }
        public virtual ICollection<FoodPreference> FoodPreferences { get; set; }
        public virtual ICollection<KitchenSupply> KitchenSupplies { get; set; }
        public virtual Credential Credential { get; set; }
    }
}