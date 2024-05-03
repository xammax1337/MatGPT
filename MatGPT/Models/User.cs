namespace MatGPT.Models
{
    public class User
    {
        public int UserId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }

        public virtual ICollection<Recipe> Recipes { get; set; }
        public virtual ICollection<Ingredient> Ingredients { get; set; }
        public virtual ICollection<FoodPreference> FoodPreferences { get; set; }
        public virtual ICollection<KitchenSupply> KitchenSupplies { get; set; }

        public virtual ICollection<Pantry> Pantries { get; set; }

        public virtual Credential Credential { get; set; }
    }
}