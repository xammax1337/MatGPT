using MatGPT.Data;
using MatGPT.Interfaces;
using MatGPT.Models;
using MatGPT.Models.Dtos;
using MatGPT.Models.ViewModels;
using MatGPT.Repository;
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
        private readonly IPantryRepository _pantryRepository;

        public PantryController(ApplicationContext context, OpenAIAPI api, IPantryRepository pantryRepository)
        {
            _context = context;
            _api = api;
            _pantryRepository = pantryRepository;
        }

        [HttpPost("AddPantry")]
        public async Task<IActionResult> AddPantryAsync(PantryDto dto, string pantryName, int userId)
        {
            try
            {
                var pantries = await _pantryRepository.AddPantryAsync(dto, pantryName, userId);

                if (pantries == null)
                {
                    return NotFound("not found");
                }

                return Ok($"Pantry {pantryName} added successfully.");

            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }
        }


        //We use GET-endpoint to list pantries, then use this endpoint to delete a pantry (and its pantryingredients)
        //Maybe frontend should show a warning so that user knows that the ingredients connected to the pantry also are deleted
        [HttpDelete("DeletePantry")]
        public async Task<IActionResult> DeletePantryAsync(int userId, string pantryName)
        {
            try
            {
                var pantries = await _pantryRepository.DeletePantryAsync(userId, pantryName);

                if (pantries == null)
                {
                    return NotFound("not found");
                }

                return Ok($"{pantryName} deleted");

            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpGet("ListPantry")]
        public async Task<IActionResult> ListUsersPantriesAsync(int userId)
        {
            try
            {
                var pantries = await _pantryRepository.ListPantriesFromUserAsync(userId);

                if (pantries == null)
                {
                    return NotFound("not found");
                }

                return Ok(pantries);

            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }

        }

        [HttpPost("AddIngredientToPantry")]
        public async Task<IActionResult> AddIngredientToPantryAsync(PantryIngredientDto dto, string ingredientName, string pantryName, int userId)
        {
            try
            {
                if (string.IsNullOrEmpty(ingredientName))
                {
                    return BadRequest("Ingredient name cannot be empty");
                }

                if (string.IsNullOrEmpty(pantryName))
                {
                    return BadRequest("Pantry name cannot be empty");
                }

                if (userId <= 0)
                {
                    return BadRequest("Invalid user ID");
                }

                await _pantryRepository.AddIngredientToPantryAsync(dto, ingredientName, pantryName, userId);

                return Ok($"{ingredientName} added to {pantryName} successfully.");

            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }

        //Use GET-endpoint to list PantryIngredients first,
        //then use this one.
        //Can we use LINQ to look up name - and then delete connection?
        [HttpDelete("DeleteIngredientFromPantry")]
        public async Task<IActionResult> DeleteIngredientFromPantryAsync(int userId, string ingredientName, string pantryName)
        {
            try
            {
                if (string.IsNullOrEmpty(ingredientName))
                {
                    return BadRequest("Ingredient name cannot be empty");
                }

                if (string.IsNullOrEmpty(pantryName))
                {
                    return BadRequest("Pantry name cannot be empty");
                }

                if (userId <= 0)
                {
                    return BadRequest("Invalid user ID");
                }
                await _pantryRepository.DeleteIngredientFromPantryAsync(userId, ingredientName, pantryName);

                return Ok($"{ingredientName} deleted from {pantryName}");
            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }
        }

        // Change and fix the error handling!!!!!!!
        [HttpGet("ListPantryIngredientsAsync")]
        public async Task<IActionResult> ListPantryIngredientsAsync(int userId, string pantryName)
        {
            try
            {
                if (string.IsNullOrEmpty(pantryName))
                {
                    return BadRequest("Pantry name cannot be empty");
                }

                if (userId <= 0)
                {
                    return BadRequest("Invalid user ID");
                }

                var listofPantryIngredients = await _pantryRepository.ListPantryIngredientsAsync(userId, pantryName);

                return Ok(listofPantryIngredients);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
            
        }
    }
}
