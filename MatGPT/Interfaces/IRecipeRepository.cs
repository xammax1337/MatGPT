using MatGPT.Models;
using MatGPT.Models.ViewModels;

namespace MatGPT.Interfaces
{
    public interface IRecipeRepository
    {
        //For generating the recipe
        Task<List<string>> GetKitchenSuppliesAsync(int userId);
        Task<List<string>> GetIngredientsAsync(int userId);
        Task<List<string>> GetPreferencesAsync(int userId);

        //For saving recipe
        Task<Recipe> GetLastRecipeAsync(int userId);
        Task<Recipe> RemoveLastRecipeAsync(int userId);
        Task<Recipe> SaveRecipeAsync(Recipe recipe);
        Task<IEnumerable<RecipeViewModel>> ListUsersRecipe(int userId);

        Task SaveChangesAsync();
    }
}
