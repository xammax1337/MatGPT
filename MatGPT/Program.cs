using MatGPT.Data;
using MatGPT.Models;
using MatGPT.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Newtonsoft.Json;
using OpenAI_API;
using OpenAI_API.Chat;
using OpenAI_API.Images;
using OpenAI_API.Models;
using Sprache;
using System.Formats.Asn1;
using static System.Net.WebRequestMethods;

var builder = WebApplication.CreateBuilder(args);

string connectionString = builder.Configuration.GetConnectionString("ApplicationContext");
builder.Services.AddDbContext<ApplicationContext>(opt => opt.UseSqlServer(connectionString));

DotNetEnv.Env.Load();

// Add services to the container.

builder.Services.AddControllers();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//Starts a new instance of openAIAPI and gives us the key from .env
builder.Services.AddSingleton(sp => new OpenAIAPI(Environment.GetEnvironmentVariable("OPENAI_API_KEY")));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.MapGet("/GenerateRecipe", async (string query, int userId, ApplicationContext dbContext,  OpenAIAPI api) =>
{
    var chat = api.Chat.CreateConversation();
    chat.Model = OpenAI_API.Models.Model.ChatGPTTurbo;
    chat.RequestParameters.Temperature = 0;

    chat.AppendSystemMessage("You will generate recipes ONLY based on the ingredients provided to you. Do not add things that are not specified as available. Only append title, ingredients, how to make the recipe and state estimated cookingtime - without extra sentences. Answer in English. Return Json in these fields: Title, ingredients, instructions and cookingtime.");

    //Filter: Will ensure that generated recipe will use these available tools
    var kitchenSupplies = await dbContext.KitchenSupply
    .Where(ks => ks.UserId == userId)
    .Select(ks => ks.KitchenSupplyName)
    .ToListAsync();

    string kSUserInput = $"I have these tools available for cooking: {string.Join(", ", kitchenSupplies)}";

    //Filter: Will ensure that generated recipe will use these available ingredients
    var pantryFoodItems = await dbContext.FoodItems
    .Where(fi => fi.UserId == userId)
    .Select(fi => fi.FoodItemName)
    .ToListAsync();

    string pFIUserInput = $"I have these ingredients in my usual pantry: {string.Join(", ", pantryFoodItems)}";

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

    
    //await GenerateImageByRecipeTitle(recipe.Title, api);

    return Results.Ok(recipe);
});




app.MapGet("/GenerateRecipeByFoodPreference", async (string query, int userId, ApplicationContext dbContext, OpenAIAPI api) =>
{
    var chat = api.Chat.CreateConversation();
    chat.Model = OpenAI_API.Models.Model.ChatGPTTurbo;
    chat.RequestParameters.Temperature = 0;
    chat.RequestParameters.ResponseFormat = ChatRequest.ResponseFormats.JsonObject;

    chat.AppendSystemMessage("You will generate recipes ONLY based on the ingredients provided to you. Do not add things that are not specified as available. Only append title, ingredients, how to make the recipe and state estimated cookingtime - without extra sentences. Answer in English. Return Json in these fields: Title, ingredients, instructions and cookingtime.");


    var kitchenSupplies = await dbContext.KitchenSupply
    .Where(ks => ks.UserId == userId)
    .Select(ks => ks.KitchenSupplyName)
    .ToListAsync();

    string kSUserInput = $"I have these tools available for cooking: {string.Join(", ", kitchenSupplies)}";

    var pantryFoodItems = await dbContext.FoodItems
    .Where(fi => fi.UserId == userId)
    .Select(fi => fi.FoodItemName)
    .ToListAsync();

    string pFIUserInput = $"I have these ingredients in my usual pantry: {string.Join(", ", pantryFoodItems)}";

    //Filter: Will ensure that generated recipe adjusts according to diets/allergies
    var foodPreference = await dbContext.FoodPreferences
    .Where(fp => fp.UserId == userId)
    .Select(fp => fp.FoodPreferenceName)
    .ToListAsync();

    string fPUserInput = $"I want a recipe that takes these allergies or diets into consideration: {string.Join(", ", foodPreference)}";


    chat.AppendUserInput(kSUserInput);

    chat.AppendUserInput(pFIUserInput);

    chat.AppendUserInput(fPUserInput);

    chat.AppendUserInput(query);


    var answer = await chat.GetResponseFromChatbotAsync();

    string jsonResponse = answer;

    var responseObject = JsonConvert.DeserializeObject<dynamic>(jsonResponse);

    Console.WriteLine(responseObject.ToString());

    var recipeJson = responseObject.ToString();

    var recipe = JsonConvert.DeserializeObject<RecipeViewModel>(recipeJson);

    //await GenerateImageByRecipeTitle(recipe.Title, api);

    return Results.Ok(recipe);
});

// static async Task GenerateImageByRecipeTitle(string recipeTitle, OpenAIAPI api)
//{
//    var result = await api.ImageGenerations.CreateImageAsync(new ImageGenerationRequest($"Food plating image of: {recipeTitle}. Image should be appealing, like an advertisement image.", OpenAI_API.Models.Model.DALLE3));

//    await Console.Out.WriteLineAsync(result.Data[0].Url);
//};


app.Run();
