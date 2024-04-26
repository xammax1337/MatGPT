using System.Text.Json.Serialization;

namespace MatGPT.Models.ViewModels
{
    public class RecipeViewModel
    {
        public string Title { get; set; }
        public List<string> Ingredients { get; set; }
        public string Instructions { get; set; }

        public string CookingTime { get; set; }
    }
}
