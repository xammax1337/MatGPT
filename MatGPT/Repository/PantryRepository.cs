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
    }
}
