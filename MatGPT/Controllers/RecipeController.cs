using MatGPT.Data;
using MatGPT.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ValueGeneration.Internal;
using Microsoft.Extensions.Caching.Memory;
using OpenAI_API;

namespace MatGPT.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class RecipeController : Controller
    {
        private readonly ApplicationContext _context;
        private readonly OpenAIAPI _api;

        public RecipeController(ApplicationContext context, OpenAIAPI api)
        {
            _context = context;
            _api = api;
        }

        [HttpPost("GenerateRecipe")]
        public async Task<IActionResult> GenerateRecipe(string query)
        {

            //var chat = _api.Chat.CreateConversation();
            //chat.Model = OpenAI_API.Models.Model.ChatGPTTurbo;
            //chat.RequestParameters.Temperature = 0;

            //chat.AppendSystemMessage("You will generate recipes ONLY based on the ingredients provided to you. Do not add things that are not specified as available. State estimated cooking time. Answer in chosen language.");

            //chat.AppendUserInput(query);

            //var answer = await chat.GetResponseFromChatbotAsync();

            // Retrieve user's ID from session
            string userId = HttpContext.Session.GetString("UserId");

            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized("User not authenticated");
            }

            //Hard coded the recipe desc for testing.
            string answer = "Cook the food";

            // gave it a temp name that stores, need a method to auto remove it if another endpoint is not contacted.
            await _context.Recipes.AddAsync(new Recipe
            {
                RecipeName = "temporary recipe name",
                RecipeDescription = answer,
                UserId = int.Parse(userId)
            });

            await _context.SaveChangesAsync();

            return new JsonResult(new { RecipeDescription = answer });
        }

        [HttpPost("SaveRecipe")]
        public async Task<IActionResult> SaveRecipe(string recipeName, bool saveRecipe)
        {
            // Retrieve user's ID from session
            string userId = HttpContext.Session.GetString("UserId");

            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized("User not authenticated");
            }

            if (saveRecipe)
            {
                if (string.IsNullOrEmpty(recipeName))
                {
                    return BadRequest("Recipe name cannot be empty.");
                }

                // Find the last saved recipe by sorting by userid and latest recipe
                var lastRecipe = await _context.Recipes
                    .Where(r => r.UserId == int.Parse(userId))
                    .OrderByDescending(r => r.RecipeId)
                    .FirstOrDefaultAsync();

                if (lastRecipe == null)
                {
                    return NotFound("No recipe to save.");
                }

                lastRecipe.RecipeName = recipeName;

                try
                {
                    await _context.SaveChangesAsync();
                    return Ok($"Saved the recipe as {recipeName}");
                }
                catch (DbUpdateException ex)
                {
                    return StatusCode(500, "An error occurred while updating the recipe. Please try again later.");
                }
            }
            else
            {
                var lastRecipe = await _context.Recipes
                    .Where(r => r.UserId == int.Parse(userId))
                    .OrderByDescending(r => r.RecipeId)
                    .FirstOrDefaultAsync();

                _context.Recipes.Remove(lastRecipe);
                await _context.SaveChangesAsync();
                return Ok("Recipe not saved, deleted from db.");
            }

        }
    }
}
