using MatGPT.Data;
using MatGPT.Interfaces;
using MatGPT.Models;
using MatGPT.Models.Dtos;
using MatGPT.Models.ViewModels;
using MatGPT.Repository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OpenAI_API;
using System.Text.RegularExpressions;
using System.Xml.Linq;

namespace MatGPT.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class PantryController : Controller
    {
        private readonly ApplicationContext _context;
        private readonly OpenAIAPI _api;
        private readonly IPantryRepository _pantryRepository;

        public PantryController(ApplicationContext context, OpenAIAPI api, IPantryRepository pantryRepository)
        {
            _context = context;
            _api = api;
            _pantryRepository = pantryRepository;
        }

        [HttpPost("AddPantry")]
        public async Task<IActionResult> AddPantryAsync(PantryDto dto, string pantryName, int userId)
        {
            await _pantryRepository.AddPantryAsync(dto, pantryName, userId);

            return Ok($"Pantry {pantryName} added successfully.");
        }


        //We use GET-endpoint to list pantries, then use this endpoint to delete a pantry (and its pantryingredients)
        //Maybe frontend should show a warning so that user knows that the ingredients connected to the pantry also are deleted
        [HttpDelete("DeletePantry")]
        public async Task<IActionResult> DeletePantryAsync(int userId, string pantryName)
        {
            await _pantryRepository.DeletePantryAsync(userId, pantryName);

            return Ok($"{pantryName} deleted");
        }

        [HttpGet("ListPantry")]
        public async Task<IActionResult> ListUsersPantriesAsync(int userId)
        {
            try
            {
                var pantries = await _pantryRepository.ListPantriesFromUserAsync(userId);

                if (pantries == null)
                {
                    return NotFound("not found");
                }

                return Ok(pantries);

            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }

        }

        [HttpPost("AddIngredientToPantry")]
        public async Task<IActionResult> AddIngredientToPantryAsync(PantryIngredientDto dto, string IngredientName, string pantryName, int userId)
        {

            var pantry = await _context.Pantries.FirstOrDefaultAsync(p => p.PantryName == pantryName);

            var ingredient = await _context.Ingredients.FirstOrDefaultAsync(f => f.IngredientName == IngredientName);

            await _context.PantryIngredients.AddAsync(new PantryIngredient { PantryId = pantry.PantryId, IngredientId = ingredient.IngredientId });

            await _context.SaveChangesAsync();

            return Ok($"{IngredientName} added to {pantryName} successfully.");
        }

        //Use GET-endpoint to list PantryIngredients first,
        //then use this one.
        //Can we use LINQ to look up name - and then delete connection?
        [HttpDelete("DeleteIngredientFromPantry")]
        public async Task<IActionResult> DeleteIngredientFromPantryAsync(int pantryIngredientId)
        {
            var ingredientToDeleteFromPantry = _context.PantryIngredients
                .SingleOrDefault(p => p.PantryIngredientId == pantryIngredientId);

            if (ingredientToDeleteFromPantry == null)
            {
                throw new Exception($"Pantry or ingredient not found");
            }

            _context.PantryIngredients.Remove(ingredientToDeleteFromPantry);
            _context.SaveChanges();
            return Ok("Ingredient deleted from pantry");
        }

        [HttpGet("ListPantryIngredientsAsync")]
        public async Task<IEnumerable<PantryIngredientDto>> ListPantryIngredientsAsync(int userId, int pantryId)
        {
            //var pantry = _context.Pantries
            //    .SingleOrDefault(p => p.PantryName.ToLower() == pantryName.ToLower() && p.UserId == userId);

            //if (pantry == null)
            //{
            //    return NotFound("Pantry not found");
            //}

            //var pantryIngredients = await _context.PantryIngredients
            //    .Where(pi => pi.PantryId == pantry.PantryId)
            //    .Select(pi => pi.Ingredient)
            //    .ToListAsync();

            //return Ok(pantryIngredients);

            var user = await _context.Users
                .Include(u => u.Pantries)
                .FirstOrDefaultAsync(u => u.UserId == userId);

            var pantry = user.Pantries.FirstOrDefault(p => p.PantryId == pantryId);

            var pantryIngredients = await _context.PantryIngredients
               .Include(pi => pi.Ingredient)
               .Where(pi => pi.PantryId == pantryId)
               .ToListAsync();

            // Check if there are any pantry ingredients
            if (pantryIngredients == null || !pantryIngredients.Any())
            {
                return Enumerable.Empty<PantryIngredientDto>();
            }

            var ingredientViewModel = pantryIngredients
                    .Select(pi => new PantryIngredientDto
                    {
                        PantryId = pi.PantryId,
                        IngredientId = pi.IngredientId,
                        IngredientName = pi.Ingredient.IngredientName // Assuming there's a property 'Name' in the Ingredient entity
                    })
                    .ToList();

            return ingredientViewModel;
        }
    }
}
