namespace MatGPT.Models
{
    public class Recipe
    {
        public int RecipeId { get; set; }
        public string RecipeName { get; set; }
        public string RecipeDescription { get; set;}
        public int CookingTime { get; set; }
        public int UserId { get; set; }

        public User User { get; set; }


    }
}
