using MatGPT.Data;
using MatGPT.Interfaces;
using MatGPT.Models;
using MatGPT.Models.ViewModels;
using MatGPT.Repository;
using MatGPT.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using OpenAI_API;
using OpenAI_API.Chat;
using OpenAI_API.Images;
using OpenAI_API.Models;
using Sprache;
using System.Formats.Asn1;
using System.Text;
using static System.Net.WebRequestMethods;

var builder = WebApplication.CreateBuilder(args);

string connectionString = builder.Configuration.GetConnectionString("ApplicationContext");

builder.Services.AddDbContext<ApplicationContext>(opt => opt.UseSqlServer(connectionString));
//Repositories
builder.Services.AddScoped<IRecipeRepository, RecipeRepository>();
builder.Services.AddScoped<IKitchenSupplyRepository, KitchenSupplyRepository>();
builder.Services.AddScoped<IFoodPreferenceRepository, FoodPreferenceRepository>();
builder.Services.AddScoped<IPantryRepository, PantryRepository>();
builder.Services.AddScoped<IIngredientRepository, IngredientRepository>();
builder.Services.AddScoped<UserService>();

DotNetEnv.Env.Load();
var jwtKey = Environment.GetEnvironmentVariable("JWT_KEY");
var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));

// Adding JWT Bearer and authentication
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
       .AddJwtBearer(options => {
           options.TokenValidationParameters = new TokenValidationParameters
           {
               ValidateIssuer = true,
               ValidateAudience = true,
               ValidateLifetime = true,
               ValidateIssuerSigningKey = true,
               ValidIssuer = builder.Configuration["Jwt:Issuer"],
               ValidAudience = builder.Configuration["Jwt:Audience"],
               IssuerSigningKey = key
           };
       });


// Add services to the container.

builder.Services.AddControllers();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigin",
        builder =>
        {
            builder.WithOrigins("http://localhost:3000/", "http://localhost:5173/", "http://127.0.0.1:5500") // Specify the origin of your frontend app
                   .AllowAnyHeader()
                   .AllowAnyMethod()
                   .AllowCredentials();
                   
        });
});

// Added session service so we can use Session to check user authorization
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(20); // Set session expiration time
});

builder.Services.AddDistributedMemoryCache();

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

app.UseCors("AllowSpecificOrigin");

app.UseSession();

app.UseAuthentication(); // Adding authentication

app.UseAuthorization();

app.MapControllers();

app.Run();
