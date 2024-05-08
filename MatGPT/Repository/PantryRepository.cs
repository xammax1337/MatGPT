using MatGPT.Data;
using MatGPT.Interfaces;
using MatGPT.Models;
using MatGPT.Models.Dtos;
using MatGPT.Models.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace MatGPT.Repository
{
    public class PantryRepository : IPantryRepository
    {
        private readonly ApplicationContext _context;
        public PantryRepository(ApplicationContext context)
        {
            _context = context;
        }

        // Add pantry to a user
        public async Task<Pantry> AddPantryAsync(PantryDto dto, string pantryName, int userId)
        {
            var pantry = new Pantry
            {
                PantryName = pantryName,
                UserId = userId,
            };

            await _context.Pantries.AddAsync(pantry);
            await _context.SaveChangesAsync();

            return pantry;
        }

        // Delete a pantry from a users storage
        public async Task<Pantry> DeletePantryAsync(int userId, string pantryName)
        {
            var pantryToDelete = _context.Pantries
                .Include(p => p.PantryIngredients)
                .FirstOrDefault(p => p.UserId == userId && p.PantryName.ToLower() == pantryName.ToLower());

            if (pantryToDelete == null)
            {
                throw new Exception($"{pantryName} not found");
            }

            //Deletes pantryingredients first
            _context.PantryIngredients.RemoveRange(pantryToDelete.PantryIngredients);
            _context.Pantries.Remove(pantryToDelete);
            await _context.SaveChangesAsync();
            return pantryToDelete;
        }

        // List pantries from user
        public async Task<IEnumerable<PantryViewModel>> ListPantriesFromUserAsync(int userId)
        {
            var user = await _context.Users
                .Include(u => u.Pantries)
                .FirstOrDefaultAsync(u => u.UserId == userId);

            if (user == null)
            {
                throw new Exception("User not found");
            }

            var pantryViewModel = user.Pantries
                .Select(p => new PantryViewModel
                {
                    PantryName = p.PantryName
                });

            return pantryViewModel;
        }

        // Connect an ingredient to a pantry for a user
        public async Task AddIngredientToPantryAsync(PantryIngredientDto dto, string ingredientName, string pantryName, int userId)
        {
            var pantry = await _context.Pantries
                .Include(p => p.User)
                .FirstOrDefaultAsync(p => p.PantryName.ToLower() == pantryName.ToLower() && p.UserId == userId);

            if (pantry == null)
            {
                // Handle the case where the pantry is not found
                throw new Exception($"Pantry '{pantryName}' not found for the specified user.");
            }

            var ingredient = await _context.Ingredients
                .Include(f => f.User)
                .FirstOrDefaultAsync(f => f.IngredientName.ToLower() == ingredientName.ToLower() && f.UserId == userId);

            if (ingredient == null)
            {
                // Handle the case where the ingredient is not found
                throw new Exception($"Ingredient '{ingredientName}' not found for the specified user.");
            }

            await _context.PantryIngredients.AddAsync(new PantryIngredient { PantryId = pantry.PantryId, IngredientId = ingredient.IngredientId });

            await _context.SaveChangesAsync();

            return;
        }

        public async Task DeleteIngredientFromPantryAsync(int userId, string ingredientName, string pantryName)
        {
            var pantry = await _context.Pantries
                .Include(p => p.User)
                .FirstOrDefaultAsync(p => p.PantryName.ToLower() == pantryName.ToLower() && p.UserId == userId);

            if (pantry == null)
            {
                // Handle the case where the pantry is not found
                throw new Exception($"Pantry '{pantryName}' not found for the specified user.");
            }

            var ingredient = await _context.Ingredients
                .Include(f => f.User)
                .FirstOrDefaultAsync(f => f.IngredientName.ToLower() == ingredientName.ToLower() && f.UserId == userId);

            if (ingredient == null)
            {
                // Handle the case where the ingredient is not found
                throw new Exception($"Ingredient '{ingredientName}' not found for the specified user.");
            }

            var pantryIngredient = await _context.PantryIngredients.FirstOrDefaultAsync(pi => pi.PantryId == pantry.PantryId && pi.IngredientId == ingredient.IngredientId);

            _context.PantryIngredients.Remove(pantryIngredient);
            await _context.SaveChangesAsync();

        }

        // List ingredients from a users pantry
        public async Task<IEnumerable<PantryIngredientDto>> ListPantryIngredientsAsync(int userId, string pantryName)
        {
            var pantry = await _context.Pantries
                .Include(p => p.User)
                .FirstOrDefaultAsync(p => p.PantryName == pantryName && p.UserId == userId);

            if (pantry == null)
            {
                throw new Exception($"Pantry '{pantryName}' not found for the specified user.");
            }

            var pantryIngredients = await _context.PantryIngredients
                .Include(pi => pi.Ingredient)
                .Where(pi => pi.PantryId == pantry.PantryId)
                .ToListAsync();

            if (pantryIngredients == null)
            {
                throw new Exception($"No Ingredients found connected to '{pantryName}' for the specified user.");
            }

            // Map the pantry ingredients to DTOs
            var pantryIngredientDtos = pantryIngredients.Select(pi => new PantryIngredientDto
            {
                IngredientName = pi.Ingredient.IngredientName,
            });

            return pantryIngredientDtos;
        }
    }
}
