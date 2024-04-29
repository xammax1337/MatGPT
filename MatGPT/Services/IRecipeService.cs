using MatGPT.Data;
using MatGPT.Models;
using MatGPT.Models.ViewModels;
using MatGPT.Repositories;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using OpenAI_API;

namespace MatGPT.Services
{
    public interface IRecipeService
    {
        Task<RecipeViewModel> GenerateRecipeAsync(string query, int userId, int minTime, int maxTime, bool choseTimer, int servings, bool chosePreferences);
    }

    public class RecipeService: IRecipeService
    {
        private readonly IRecipeRepository _recipeRepository;
        private readonly OpenAIAPI _api;
        private readonly ApplicationContext _context;
        public RecipeService(IRecipeRepository recipeRepository, OpenAIAPI api, ApplicationContext context)
        {
            _recipeRepository = recipeRepository;
            _api = api;
            _context = context;
        }

        public async Task<RecipeViewModel> GenerateRecipeAsync(string query, int userId, int minTime, int maxTime, bool choseTimer, int servings, bool chosePreferences)
        {
            var chat = _api.Chat.CreateConversation();
            chat.Model = OpenAI_API.Models.Model.ChatGPTTurbo;
            chat.RequestParameters.Temperature = 0;

            chat.AppendSystemMessage("You will generate recipes ONLY based on the ingredients provided to you. Do not add things that are not specified as available. Only append title, ingredients, how to make the recipe and state estimated cookingtime - without extra sentences. Answer in English. Return Json in these fields: Title, instructions, ingredients as String and cookingtime as Int.");

            //Filter: Will ensure that generated recipe will use these available tools
            var kitchenSupplies = await _context.KitchenSupply
            .Where(ks => ks.UserId == userId)
            .Select(ks => ks.KitchenSupplyName)
            .ToListAsync();

            string kSUserInput = $"I have these tools available for cooking: {string.Join(", ", kitchenSupplies)}";

            //Filter: Will ensure that generated recipe will use these available ingredients
            var pantryIngredients = await _context.Ingredients
            .Where(fi => fi.UserId == userId)
            .Select(fi => fi.IngredientName)
            .ToListAsync();

            string pFIUserInput = $"I have these ingredients in my usual pantry: {string.Join(", ", pantryIngredients)}";

            //Filter: Tells AI to generate recipe according to time input
            if (choseTimer)
            {
                string cTUserInput = $"I want a recipe with cooking time between {minTime}-{maxTime} minutes.";
                chat.AppendUserInput(cTUserInput);
            }

            string sUserInput = $"I want {servings} servings";

            //Filter: Will ensure that generated recipe adjusts according to diets/allergies
            if (chosePreferences)
            {
                var foodPreference = await _context.FoodPreferences
                .Where(fp => fp.UserId == userId)
                .Select(fp => fp.FoodPreferenceName)
                .ToListAsync();

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


            //string imageUrl = await GenerateImageByRecipeTitle(recipe.Title, _api);

            //Save the recipe to database Temporarily
            await _context.Recipes.AddAsync(new Recipe
            {
                Title = recipe.Title,
                Instructions = recipe.Instructions,
                Ingredients = recipe.Ingredients,
                CookingTime = recipe.CookingTime,
                UserId = userId
            });

            await _context.SaveChangesAsync();

            return recipe;
        }
    }
}
