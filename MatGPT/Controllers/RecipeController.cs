using MatGPT.Data;
using MatGPT.Models;
using MatGPT.Models.ViewModels;
using MatGPT.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ValueGeneration.Internal;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;
using OpenAI_API;
using OpenAI_API.Images;

namespace MatGPT.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class RecipeController : Controller
    {
        private readonly IRecipeService _recipeService;

        public RecipeController(IRecipeService recipeService)
        {
            _recipeService = recipeService;
        }
        //This endpoint generates a recipe. We input ingredients, tools, food preferences etc
        [HttpPost("GenerateRecipe")]
        public async Task<IActionResult> GenerateRecipeAsync(string query, int userId, int minTime, int maxTime, bool choseTimer, int servings, bool chosePreferences)
        {
            var recipe = await _recipeService.GenerateRecipeAsync(query, userId, minTime, maxTime, choseTimer, servings, chosePreferences);

            // Combine the recipe and image URL into a single object
            var result = new { Recipe = recipe};

            return Ok(result);
        }

        // Image generation Method

        //static async Task<string> GenerateImageByRecipeTitle(string recipeTitle, OpenAIAPI api)
        //{
        //    var result = await api.ImageGenerations.CreateImageAsync(new ImageGenerationRequest($"Food plating image of: {recipeTitle}. Image should be appealing, like an advertisement image.", OpenAI_API.Models.Model.DALLE3));

        //    return result.Data[0].Url;
        //}


        // This endpoint NEEDS to be called after the other ones.
        // The other endpoints will auto save recipes in "temporary storage" and then this decides if the recipe should be deleted
        // or permanently saved to a user.
        //[HttpPost("SaveRecipe")]
        //public async Task<IActionResult> SaveRecipeAsync(string recipeName, bool saveRecipe)
        //{
        //    // Retrieve user's ID from session
        //    string userId = HttpContext.Session.GetString("UserId");

        //    if (string.IsNullOrEmpty(userId))
        //    {
        //        return Unauthorized("User not authenticated");
        //    }

        //    if (saveRecipe)
        //    {
        //        if (string.IsNullOrEmpty(recipeName))
        //        {
        //            return BadRequest("Recipe name cannot be empty.");
        //        }

        //        // Find the last saved recipe by sorting by userid and latest recipe
        //        var lastRecipe = await _context.Recipes
        //            .Where(r => r.UserId == int.Parse(userId))
        //            .OrderByDescending(r => r.RecipeId)
        //            .FirstOrDefaultAsync();

        //        if (lastRecipe == null)
        //        {
        //            return NotFound("No recipe to save.");
        //        }

        //        lastRecipe.Title = recipeName;

        //        try
        //        {
        //            await _context.SaveChangesAsync();
        //            return Ok($"Saved the recipe as {recipeName}");
        //        }
        //        catch (DbUpdateException ex)
        //        {
        //            return StatusCode(500, "An error occurred while updating the recipe. Please try again later.");
        //        }
        //    }
        //    else
        //    {
        //        var lastRecipe = await _context.Recipes
        //            .Where(r => r.UserId == int.Parse(userId))
        //            .OrderByDescending(r => r.RecipeId)
        //            .FirstOrDefaultAsync();

        //        _context.Recipes.Remove(lastRecipe);
        //        await _context.SaveChangesAsync();
        //        return Ok("Recipe not saved, deleted from db.");
        //    }

        //}
    }
}
