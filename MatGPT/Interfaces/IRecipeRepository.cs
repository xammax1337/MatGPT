using MatGPT.Models;

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

        Task SaveChangesAsync();
    }
}
