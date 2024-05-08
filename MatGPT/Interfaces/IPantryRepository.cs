using MatGPT.Models;
using MatGPT.Models.Dtos;
using MatGPT.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace MatGPT.Interfaces
{
    public interface IPantryRepository
    {
        Task<Pantry> AddPantryAsync(PantryDto dto, string pantryName, int userId);
        Task<Pantry> DeletePantryAsync(int userId, string pantryName);
        Task<IEnumerable<PantryViewModel>> ListPantriesFromUserAsync(int userId);

        // PantryIngredients methods
        Task AddIngredientToPantryAsync(PantryIngredientDto dto, string ingredientName, string pantryName, int userId);
        Task<IEnumerable<PantryIngredientDto>> ListPantryIngredientsAsync(int userId, string pantryName);
        Task DeleteIngredientFromPantryAsync(int userId, string ingredientName, string pantryName);
    }
}
