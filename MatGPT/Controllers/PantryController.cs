using MatGPT.Data;
using MatGPT.Models;
using MatGPT.Models.Dtos;
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

            public PantryController (ApplicationContext context, OpenAIAPI api)
            {
                _context = context;
                _api = api;
            }

            [HttpPost("AddPantry")]
            public async Task<IActionResult> AddToPantry(PantryDto dto, string pantryName, int userId)
            {
                await _context.Pantries.AddAsync(new Pantry
                {
                    PantryName = pantryName,
                    
                    UserId = userId,
                });

                await _context.SaveChangesAsync();

                return Ok($"Pantry {pantryName} added successfully.");
            }

            [HttpPost("AddIngredientToPantry")]
            public async Task<IActionResult> AddIngredientToPantry(PantryIngredientDto dto, string IngredientName, string pantryName, int userId)
            {
                
               var pantry = await _context.Pantries.FirstOrDefaultAsync(p => p.PantryName == pantryName);

               var ingredient = await _context.Ingredients.FirstOrDefaultAsync(f => f.IngredientName == IngredientName);

               await _context.PantryIngredients.AddAsync (new PantryIngredient { PantryId = pantry.PantryId, IngredientId = ingredient.IngredientId });

               await _context.SaveChangesAsync();

               return Ok($"Pantry {pantryName} added successfully.");
            }

            [HttpPost("AddIngredient")]
            public async Task<IActionResult> AddIngredient(IngredientDto dto, string ingredientName,  int userId)
            {

                await _context.Ingredients.AddAsync(new Ingredient
                {
                    IngredientName = ingredientName,

                    UserId = userId,
                });

                await _context.SaveChangesAsync();

                return Ok($"Ingredient {ingredientName} added successfully.");
            }
        }
}
