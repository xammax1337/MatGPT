using MatGPT.Data;
using MatGPT.Models;
using MatGPT.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ValueGeneration.Internal;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;
using OpenAI_API;
using OpenAI_API.Images;
using MatGPT.Repository;
using MatGPT.Interfaces;
using MatGPT.Services;
using Microsoft.AspNetCore.Authorization;

namespace MatGPT.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class RecipeController : Controller
    {
        private readonly ApplicationContext _context;
        private readonly OpenAIAPI _api;

        private readonly IRecipeRepository _recipeRepository;
        private readonly UserService _userService;

        public RecipeController(ApplicationContext dbContext, OpenAIAPI api, IRecipeRepository recipeRepository, UserService userService)
        {
            _recipeRepository = recipeRepository;
            _context = dbContext;
            _api = api;
            _userService = userService;
        }

        //This endpoint generates a recipe. We input ingredients, tools, food preferences etc
        [HttpGet("GenerateRecipe")]
        public async Task<IActionResult> GenerateRecipeAsync(string query, int userId, int minTime, int maxTime, bool chooseTimer, int servings, bool choosePreferences)
        {
            try
            {
                var chat = _api.Chat.CreateConversation();
                chat.Model = OpenAI_API.Models.Model.ChatGPTTurbo;
                chat.RequestParameters.Temperature = 0;

                chat.AppendSystemMessage("You will generate recipes ONLY based on the ingredients provided to you. Do not add things that are not specified as available. Only append title, ingredients, how to make the recipe and state estimated cookingtime - without extra sentences. Answer in English. Return Json in these fields: Title, instructions, ingredients as String and cookingtime as Int.");


                //Checks if user exists in database
                var userExists = await _recipeRepository.UserExistsAsync(userId);

                if (!userExists)
                {
                    return NotFound("User not found");
                }

                //Filter: Will ensure that generated recipe will use these available tools
                var kitchenSupplies = await _recipeRepository.GetKitchenSuppliesAsync(userId);

                string kSUserInput = $"I have these tools available for cooking: {string.Join(", ", kitchenSupplies)}";

                //Filter: Will ensure that generated recipe will use these available ingredients
                var pantryIngredients = await _recipeRepository.GetIngredientsAsync(userId);

                string pFIUserInput = $"I have these ingredients in my usual pantry: {string.Join(", ", pantryIngredients)}";

                //Filter: Tells AI to generate recipe according to time input
                if (chooseTimer)
                {
                    string cTUserInput = $"I want a recipe with cooking time between {minTime}-{maxTime} minutes.";
                    chat.AppendUserInput(cTUserInput);
                }

                string sUserInput = $"I want {servings} servings";

                //Filter: Will ensure that generated recipe adjusts according to diets/allergies
                if (choosePreferences)
                {
                    var foodPreference = await _recipeRepository.GetPreferencesAsync(userId);

                    string fPUserInput = $"I want a recipe that takes these allergies or diets into consideration: {string.Join(", ", foodPreference)}";
                }

                chat.AppendUserInput(kSUserInput);

                chat.AppendUserInput(pFIUserInput);


                chat.AppendUserInput(query);


                var answer = await chat.GetResponseFromChatbotAsync();

                //Json-answer from AI
                string jsonResponse = answer;

                //Answer is turned into dynamic object - not needing to know its exact structure
                var responseObject = JsonConvert.DeserializeObject<dynamic>(jsonResponse);

                //Shows result in console for debugging
                Console.WriteLine(responseObject.ToString());

                //Converting response object into string, assign to recipeJson. Preparation for deseralisation into strong typing object
                var recipeJson = responseObject.ToString();

                // Deserialize Json-recipe information into object of RecipeViewModel
                var recipe = JsonConvert.DeserializeObject<RecipeViewModel>(recipeJson);

                //Generate image and get the URL link
                //string imageUrl = await GenerateImageByRecipeTitle(recipe.Title, _api);

                //Automatically save the recipe to database Temporarily (Later call the SaveRecipe Endpoint to delete or save permanently
                //Bugged so didnt automatically save before, had to move this back here instead of having it on repository class.
                await _context.Recipes.AddAsync(new Recipe
                {
                    Title = recipe.Title,
                    Instructions = recipe.Instructions,
                    Ingredients = recipe.Ingredients,
                    CookingTime = recipe.CookingTime,
                    UserId = userId
                });

                await _context.SaveChangesAsync();

                // Combine the recipe and image URL into a single object
                var result = new { Recipe = recipe/*, ImageUrl = imageUrl */};

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Error in handling request");
            }


        }

        //Image generation method - one image will be generated

        //static async Task<string> GenerateImageByRecipeTitle(string recipeTitle, OpenAIAPI api)
        //{
        //    try
        //    {
        //        var result = await api.ImageGenerations.CreateImageAsync(new ImageGenerationRequest($"Food plating image of: {recipeTitle}. Image should be appealing, like an advertisement image.", OpenAI_API.Models.Model.DALLE3));

        //        return result.Data[0].Url;
        //    }
        //    catch (Exception ex)
        //    {
        //        return null;
        //    }
        //}


        // This endpoint NEEDS to be called after the other ones.
        // The other endpoints will auto save recipes in "temporary storage" and then this decides if the recipe should be deleted
        // or permanently saved to a user.
        [HttpPost("SaveRecipe")]
        [Authorize]
        public async Task<IActionResult> SaveOrRemoveRecipeAsync(string recipeName, bool saveRecipe)
        {
            // Get userId from token using method inside UserServices
            int userId = _userService.ValidateUserId(User);

            if (saveRecipe)
            {
                if (string.IsNullOrEmpty(recipeName))
                {
                    return BadRequest("Recipe name cannot be empty.");
                }

                // Find the last saved recipe by sorting by userid and latest recipe
                var lastRecipe = await _recipeRepository.GetLastRecipeAsync(userId);

                if (lastRecipe == null)
                {
                    return NotFound("No recipe to save.");
                }

                lastRecipe.Title = recipeName;

                try
                {
                    await _recipeRepository.SaveChangesAsync();
                    return Ok($"Saved the recipe as {recipeName}");
                }
                catch (DbUpdateException ex)
                {
                    return StatusCode(500, "An error occurred while updating the recipe. Please try again later.");
                }
            }
            else
            {
                var lastRecipe = await _recipeRepository.RemoveLastRecipeAsync(userId);

                if (lastRecipe == null)
                {
                    return NotFound("No recipe to delete");
                }

                return Ok("Recipe not saved, deleted from database.");
            }
        }

        [HttpGet("ListRecipe")]
        public async Task<IEnumerable<RecipeViewModel>> ListUsersRecipe(int userId)
        {
            var recipes = await _recipeRepository.ListUsersRecipe(userId);
            return (recipes);
        }

        [HttpPost("TestGenerateRecipe")]
        public async Task<IActionResult> TestGenerateRecipeAsync(string query, int userId, int minTime, int maxTime, bool chooseTimer, int servings, bool choosePreferences)
        {
            //Filter: Will ensure that generated recipe will use these available tools
            var kitchenSupplies = await _recipeRepository.GetKitchenSuppliesAsync(userId);

            string kSUserInput = $"I have these tools available for cooking: {string.Join(", ", kitchenSupplies)}";

            //Filter: Will ensure that generated recipe will use these available ingredients
            var pantryIngredients = await _recipeRepository.GetIngredientsAsync(userId);

            string pFIUserInput = $"I have these ingredients in my usual pantry: {string.Join(", ", pantryIngredients)}";

            //Filter: Tells AI to generate recipe according to time input
            if (chooseTimer)
            {
                string cTUserInput = $"I want a recipe with cooking time between {minTime}-{maxTime} minutes.";

            }

            string sUserInput = $"I want {servings} servings";

            //Filter: Will ensure that generated recipe adjusts according to diets/allergies
            if (choosePreferences)
            {
                var foodPreference = await _recipeRepository.GetPreferencesAsync(userId);

                string fPUserInput = $"I want a recipe that takes these allergies or diets into consideration: {string.Join(", ", foodPreference)}";
            }

            var recipe = new RecipeViewModel
            {
                Title = "Chicken Pasta with Tomato Sauce",
                Ingredients = "Chicken, Tomato, Pasta",
                Instructions = "1. Cook the pasta according to package instructions. 2. In a frying pan, cook the chicken until browned. 3. Add chopped tomatoes to the chicken and cook until they soften. 4. Mix in the cooked pasta and simmer for a few minutes. 5. Serve hot.",
                CookingTime = 30
            };

            string imageUrl = "https://images.unsplash.com/photo-1546069901-ba9599a7e63c?q=80&w=2680&auto=format&fit=crop&ixlib=rb-4.0.3&ixid=M3wxMjA3fDB8MHxwaG90by1wYWdlfHx8fGVufDB8fHx8fA%3D%3D";

            //Automatically save the recipe to database Temporarily (Later call the SaveRecipe Endpoint to delete or save permanently
            //await _recipeRepository.SaveRecipeAsync(recipe);

            // Combine the recipe and image URL into a single object
            var result = new { Recipe = recipe, ImageUrl = imageUrl };

            return Ok(result);
        }
    }
}
