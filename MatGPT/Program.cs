using MatGPT.Data;
using MatGPT.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using OpenAI_API;
using OpenAI_API.Models;
using System.Formats.Asn1;

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

app.MapGet("/GenerateRecipe", async (string query, string language, int userId, ApplicationContext dbContext,  OpenAIAPI api) =>
{
    var chat = api.Chat.CreateConversation();
    chat.Model = OpenAI_API.Models.Model.ChatGPTTurbo;
    chat.RequestParameters.Temperature = 0;

    chat.AppendSystemMessage($"Language: {language}. Query: {query}.");

    chat.AppendSystemMessage("You will generate recipes ONLY based on the ingredients provided to you. Do not add things that are not specified as available. State estimated cooking time. Answer in chosen language.");


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

    chat.AppendUserInput(kSUserInput);

    chat.AppendUserInput(pFIUserInput);

    chat.AppendUserInput(query);



    //chat.AppendUserInput("Cheese, tomato, ground beef");
    //chat.AppendExampleChatbotOutput("Title: Tomato Beef Melt\r\n\r\nIngredients:\r\n\r\n1 pound ground beef\r\n2 large tomatoes, sliced\r\n1 cup shredded cheese (cheddar or mozzarella)\r\nSalt and pepper to taste\r\nInstructions:\r\n\r\nPreheat oven to 375°F (190°C).\r\nBrown ground beef in a skillet over medium heat. Season with salt and pepper.\r\nLayer half of the sliced tomatoes in a baking dish.\r\nSpread cooked ground beef over tomatoes.\r\nTop with remaining tomatoes and shredded cheese.\r\nBake for 20-25 minutes until cheese is melted and bubbly.\r\nServe hot and enjoy!\r\nThis recipe is quick and easy, perfect for a tasty meal with just a few simple ingredients.\r\nEstimated cooking time is 20-25 minutes.");
    //chat.AppendUserInput("Heavy cream, broccoli, pork chop");
    //chat.AppendExampleChatbotOutput("Title: Creamy Pork Chop with Broccoli\r\n\r\nIngredients:\r\n\r\n4 pork chops\r\n2 cups broccoli florets\r\n1 cup heavy cream\r\nSalt and pepper to taste\r\n2 tablespoons olive oil\r\n2 cloves garlic, minced (optional)\r\n1 teaspoon dried thyme (optional)\r\nChopped fresh parsley for garnish (optional)\r\nInstructions:\r\n\r\nPreheat oven to 375°F (190°C).\r\nSeason pork chops with salt and pepper on both sides.\r\nHeat olive oil in a skillet over medium-high heat. Add pork chops and sear until golden brown on both sides, about 3-4 minutes per side. Remove from skillet and set aside.\r\nIn the same skillet, add minced garlic (if using) and sauté for about 1 minute until fragrant.\r\nAdd broccoli florets to the skillet and cook for 2-3 minutes until slightly tender.\r\nReduce heat to medium-low and pour in heavy cream. Stir well to combine with the broccoli.\r\nReturn the pork chops to the skillet, nestling them into the creamy broccoli mixture.\r\nIf using dried thyme, sprinkle it over the pork chops.\r\nCover the skillet with a lid or aluminum foil and transfer it to the preheated oven.\r\nBake for 20-25 minutes until the pork chops are cooked through and the broccoli is tender.\r\nRemove from oven and let it rest for a few minutes.\r\nGarnish with chopped fresh parsley if desired.\r\nServe hot and enjoy your creamy pork chops with broccoli!\r\nThe estimated cooking time for this recipe is approximately 30-35 minutes, including preparation and baking time. Adjust the cooking time as needed based on the thickness of the pork chops and your desired level of doneness.");
    

    var answer = await chat.GetResponseFromChatbotAsync();

    return new JsonResult(answer);
});




app.Run();
