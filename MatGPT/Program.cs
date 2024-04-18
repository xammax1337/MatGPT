using MatGPT.Data;
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

app.MapGet("/GenerateRecipe", async (string query, string language,  OpenAIAPI api) =>
{
    var chat = api.Chat.CreateConversation();
    chat.Model = OpenAI_API.Models.Model.ChatGPTTurbo;
    chat.RequestParameters.Temperature = 0;

    chat.AppendSystemMessage($"Language: {language}. Query: {query}");

    chat.AppendSystemMessage("You will generate recipes ONLY based on the ingredients provided to you. Do not add things that are not specified as available.");


    chat.AppendSystemMessage("State estimated cooking time. Answer in chosen language.");



    chat.AppendUserInput(query);

    var answer = await chat.GetResponseFromChatbotAsync();

    return new JsonResult(answer);
});




app.Run();
