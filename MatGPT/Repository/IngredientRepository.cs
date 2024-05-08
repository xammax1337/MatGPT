using MatGPT.Data;
using MatGPT.Interfaces;
using MatGPT.Models.Dtos;
using MatGPT.Models.ViewModels;
using MatGPT.Models;
using Microsoft.EntityFrameworkCore;

namespace MatGPT.Repository
{
    public class IngredientRepository : IIngredientRepository
    {
        private readonly ApplicationContext _context;
        public IngredientRepository(ApplicationContext context) 
        {
            _context = context;
        }
        public async Task<Ingredient> AddIngredientAsync(IngredientDto dto, string ingredientName, int userId)
        {
            var ingredient = new Ingredient
            {
                IngredientName = ingredientName,
                UserId = userId,
            };

            await _context.Ingredients.AddAsync(ingredient);
            await _context.SaveChangesAsync();

            return ingredient;
        }

        public async Task<Ingredient> DeleteIngredientAsync(int userId, string ingredientName)
        {
            var ingredientToDelete = _context.Ingredients
                .FirstOrDefault(i => i.UserId == userId && i.IngredientName.ToLower() == ingredientName.ToLower());

            if (ingredientToDelete == null)
            {
                throw new Exception($"{ingredientName} not found");
            }

            _context.Ingredients.Remove(ingredientToDelete);

            await _context.SaveChangesAsync();
            return ingredientToDelete;
        }

        public async Task<IEnumerable<IngredientViewModel>> ListIngredientsFromUserAsync(int userId)
        {
            var user = await _context.Users
                .Include(u => u.Ingredients)
                .FirstOrDefaultAsync(u => u.UserId == userId);

            if (user == null)
            {
                throw new Exception("User not found");
            }

            var ingredientViewModel = user.Ingredients
                .Select(p => new IngredientViewModel
                {
                    IngredientName = p.IngredientName
                });

            return ingredientViewModel;
        }
    }
}
