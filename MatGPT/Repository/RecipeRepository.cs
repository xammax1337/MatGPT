using MatGPT.Data;
using MatGPT.Interfaces;
using MatGPT.Models;
using MatGPT.Models.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace MatGPT.Repository
{
    public class RecipeRepository : IRecipeRepository
    {
        private readonly ApplicationContext _context;
        public RecipeRepository(ApplicationContext context)
        {
            _context = context;
        }

        public async Task<bool> UserExistsAsync(int userId)
        {
            var user = await _context.Users.FindAsync(userId);
            return user != null;
        }
        public async Task<List<string>> GetIngredientsAsync(int userId)
        {
            return await _context.Ingredients
                .Where(i => i.UserId == userId)
                .Select(i => i.IngredientName)
                .ToListAsync();
        }

        public async Task<List<string>> GetKitchenSuppliesAsync(int userId) // QUYNH TAR ÖVER, RÖR EJ!!!
        {
            try
            {
                var userExists = await UserExistsAsync(userId);
                if (!userExists)
                {
                    throw new Exception("User not found");
                }

                var kitchenSupplies = await _context.KitchenSupplies
                    .Where(ks => ks.UserId == userId)
                    .Select(ks => ks.KitchenSupplyName)
                    .ToListAsync();

                if (kitchenSupplies == null || kitchenSupplies.Count == 0)
                {
                    throw new Exception("No kitchen supplies found for the specified user.");
                }

                return kitchenSupplies;
            }
            catch (Exception ex)
            {
                throw new Exception("Error when handling request");
            }
        }

        public async Task<List<string>> GetPreferencesAsync(int userId)
        {
            try
            {
                var userExists = await UserExistsAsync(userId);
                if (!userExists)
                {
                    throw new Exception("User not found");
                }

                var foodPreferences = await _context.FoodPreferences
                    .Where(fp => fp.UserId == userId)
                    .Select(fp => fp.FoodPreferenceName)
                    .ToListAsync();

                return foodPreferences;
            }
            catch (Exception ex)
            {
                throw new Exception("Error when handling request");
            }

        }

        public async Task<Recipe> GetLastRecipeAsync(int userId)
        {
            return await _context.Recipes
                .Where(r => r.UserId == userId)
                .OrderByDescending(r => r.RecipeId)
                .FirstOrDefaultAsync();
        }

        public async Task<Recipe> RemoveLastRecipeAsync(int userId)
        {
            var lastRecipe = await GetLastRecipeAsync(userId);

            if (lastRecipe != null)
            {
                _context.Recipes.Remove(lastRecipe);
                await _context.SaveChangesAsync();
            }

            return lastRecipe;
        }

        public async Task<Recipe> SaveRecipeAsync(Recipe recipe)
        {
            var newRecipe = new Recipe
            {
                Title = recipe.Title,
                Instructions = recipe.Instructions,
                Ingredients = recipe.Ingredients,
                CookingTime = recipe.CookingTime,
                UserId = recipe.UserId
            };

            await _context.Recipes.AddAsync(newRecipe);
            await _context.SaveChangesAsync();

            return newRecipe;
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<RecipeViewModel>> ListUsersRecipe(int userId)
        {
            var user = await _context.Users
                .Include(u => u.Recipes)
                .FirstOrDefaultAsync(u => u.UserId == userId);

            if (user == null)
            {
                throw new Exception("User not found");
            }

            var recipeViewModel = user.Recipes
                .Select(r => new RecipeViewModel
                {
                    Title = r.Title,
                    Instructions = r.Instructions,
                    Ingredients = r.Ingredients,
                    CookingTime = r.CookingTime
                });

            return recipeViewModel;
        }
    }
}