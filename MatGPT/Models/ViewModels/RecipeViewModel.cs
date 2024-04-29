using System.Text.Json.Serialization;

namespace MatGPT.Models.ViewModels
{
    public class RecipeViewModel
    {
        public string Title { get; set; }
        public string Ingredients { get; set; }
        public string Instructions { get; set; }

        public int? CookingTime { get; set; }
    }
}
