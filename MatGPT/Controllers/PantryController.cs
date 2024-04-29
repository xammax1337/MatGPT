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

            [HttpPost("AddFoodItemToPantry")]
            public async Task<IActionResult> AddFoodItemToPantry(PantryFoodItemDto dto, string foodItemName, string pantryName, int userId)
            {
                
               var pantry = await _context.Pantries.FirstOrDefaultAsync(p => p.PantryName == pantryName);

               var foodItem = await _context.FoodItems.FirstOrDefaultAsync(f => f.FoodItemName == foodItemName);

               await _context.PantryFoodItems.AddAsync (new PantryFoodItem { PantryId = pantry.PantryId, FoodItemId = foodItem.FoodItemId });

               await _context.SaveChangesAsync();

               return Ok($"Pantry {pantryName} added successfully.");
            }

            [HttpPost("AddFoodItem")]
            public async Task<IActionResult> AddFoodItem(FoodItemDto dto, string foodItemName,  int userId)
            {

                await _context.FoodItems.AddAsync(new FoodItem
                {
                    FoodItemName= foodItemName,

                    UserId = userId,
                });

                await _context.SaveChangesAsync();

                return Ok($"FoodItem {foodItemName} added successfully.");
            }
        }
}
