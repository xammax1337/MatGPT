using MatGPT.Models;
using MatGPT.Models.Dtos;
using MatGPT.Models.ViewModels;

namespace MatGPT.Interfaces
{
    public interface IIngredientRepository
    {
        Task<Ingredient> AddIngredientAsync(IngredientDto dto, string ingredientName, int userId);
        Task<Ingredient> DeleteIngredientAsync(int userId, string ingredientName);
        Task<IEnumerable<IngredientViewModel>> ListIngredientsFromUserAsync(int userId);
    }
}
