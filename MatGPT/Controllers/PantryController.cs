using MatGPT.Data;
using MatGPT.Models;
using MatGPT.Models.Dtos;
using MatGPT.Models.ViewModels;
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

        public PantryController(ApplicationContext context, OpenAIAPI api)
        {
            _context = context;
            _api = api;
        }

        [HttpPost("AddPantry")]
        public async Task<IActionResult> AddPantryAsync(PantryDto dto, string pantryName, int userId)
        {
            await _context.Pantries.AddAsync(new Pantry
            {
                PantryName = pantryName,

                UserId = userId,
            });

            await _context.SaveChangesAsync();

            return Ok($"Pantry {pantryName} added successfully.");
        }


        //We use GET-endpoint to list pantries, then use this endpoint to delete a pantry (and its pantryingredients)
        //Maybe frontend should show a warning so that user knows that the ingredients connected to the pantry also are deleted
        [HttpDelete("DeletePantry")]
        public async Task<IActionResult> DeletePantryAsync(int userId, string pantryName)
        {
            var pantryToDelete = _context.Pantries
                .Include(p => p.PantryIngredients)
                .FirstOrDefault(p => p.UserId == userId && p.PantryName.ToLower() == pantryName.ToLower());

            if (pantryToDelete == null)
            {
                return NotFound($"{pantryName} not found");
            }

            //Deletes pantryingredients first
            _context.PantryIngredients.RemoveRange(pantryToDelete.PantryIngredients);
            _context.Pantries.Remove(pantryToDelete);
            await _context.SaveChangesAsync();
            return Ok($"{pantryName} deleted");
        }

        [HttpGet("ListPantry")]
        public async Task<IActionResult> ListUsersPantriesAsync(int userId)
        {
            User? user = await _context.Users
                 .Include(u => u.Pantries)
                 .SingleOrDefaultAsync(u => u.UserId == userId);

            if (user == null)
            {
                throw new Exception("User or pantry not found");
            }

   
            List<PantryViewModel> result = user.Pantries
                .Select(p => new PantryViewModel()
                {
                    PantryName = p.PantryName,
                }).ToList();

            return Ok(result);

        }



        [HttpPost("AddIngredientToPantry")]
        public async Task<IActionResult> AddIngredientToPantryAsync(PantryIngredientDto dto, string IngredientName, string pantryName, int userId)
        {

            var pantry = await _context.Pantries.FirstOrDefaultAsync(p => p.PantryName == pantryName);

            var ingredient = await _context.Ingredients.FirstOrDefaultAsync(f => f.IngredientName == IngredientName);

            await _context.PantryIngredients.AddAsync(new PantryIngredient { PantryId = pantry.PantryId, IngredientId = ingredient.IngredientId });

            await _context.SaveChangesAsync();

            return Ok($"Pantry {pantryName} added successfully.");
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


        [HttpPost("AddIngredient")]
        public async Task<IActionResult> AddIngredientAsync(IngredientDto dto, string ingredientName, int userId)
        {

            await _context.Ingredients.AddAsync(new Ingredient
            {
                IngredientName = ingredientName,

                UserId = userId,
            });

            await _context.SaveChangesAsync();

            return Ok($"Ingredient {ingredientName} added successfully.");
        }

        //In order to use the DELETEs: We have to use the GET that lists the items first,
        //then we use the DELETE
        [HttpDelete("DeleteIngredient")]
        public async Task<IActionResult> DeleteIngredientAsync(int userId, string ingredientName)
        {
            var ingredientToDelete = _context.Ingredients
                .FirstOrDefault(i => i.UserId == userId && i.IngredientName.ToLower() == ingredientName.ToLower());

            if (ingredientToDelete == null)
            {
                return NotFound($"{ingredientName} not found");
            }

            _context.Ingredients.Remove(ingredientToDelete);

            await _context.SaveChangesAsync();

            return Ok($"{ingredientName} deleted");
        }

        [HttpGet("ListIngredients")]
        public async Task<IActionResult> ListUsersIngredientsAsync(int userId)
        {
            User? user = await _context.Users
                .Include(u => u.Ingredients)//fel namn i databas!!!!!!
                .SingleOrDefaultAsync(u => u.UserId == userId);

            if (user == null)
            {
                throw new Exception("User or ingredient not found");
            }

            List<IngredientViewModel> result = user.Ingredients
                .Select(i => new IngredientViewModel()
                {
                    IngredientName = i.IngredientName
                }).ToList();

            return Ok(result);
        }


        [HttpGet("ListPantryIngredientsAsync")]
        public async Task<IActionResult> ListPantryIngredientsAsync(int userId, string pantryName)
        {
            var pantry = _context.Pantries
                .SingleOrDefault(p => p.PantryName == pantryName && p.UserId == userId);

            if (pantry == null)
            {
                return NotFound("Pantry not found");
            }

            var pantryIngredients = await _context.PantryIngredients
                .Where(pi => pi.PantryId == pantry.PantryId)
                .Select(pi => pi.Ingredient)
                .ToListAsync();

            return Ok(pantryIngredients);
        }
    }
}
