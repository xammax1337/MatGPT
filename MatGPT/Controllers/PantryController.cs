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

        [HttpPost("AddPantry")] //NOAS NYA
        public async Task<IActionResult> AddPantryAsync(PantryDto dto, string pantryName, int userId)
        {
            try
            {
                var pantry = await _pantryRepository.AddPantryAsync(dto, pantryName, userId);

                if (pantry == null)
                {
                    // Om pantry är null betyder det att något gick fel vid tilläggandet
                    throw new Exception($"Failed to add pantry '{pantryName}' for user {userId}.");
                }

                return Ok($"Pantry '{pantryName}' added successfully.");
            }
            catch (Exception ex)
            {
                // Logga felmeddelandet för att spåra felet
                Console.WriteLine($"An error occurred while adding pantry '{pantryName}' for user {userId}: {ex.Message}");

                // Returnera ett specifikt felmeddelande till användaren
                return StatusCode(500, "An error occurred while processing the request. Please try again later.");
            }
        }


        //[HttpPost("AddPantry")]        NOAS UTKOMMENTERADE
        //public async Task<IActionResult> AddPantryAsync(PantryDto dto, string pantryName, int userId)
        //{
        //    try
        //    {
        //        var pantries = await _pantryRepository.AddPantryAsync(dto, pantryName, userId);

        //        if (pantries == null)
        //        {
        //            return NotFound("not found");
        //        }

        //        return Ok($"Pantry {pantryName} added successfully.");

        //    }
        //    catch (Exception ex)
        //    {
        //        return NotFound(ex.Message);
        //    }
        //}


        //We use GET-endpoint to list pantries, then use this endpoint to delete a pantry (and its pantryingredients)
        //Maybe frontend should show a warning so that user knows that the ingredients connected to the pantry also are deleted


        [HttpDelete("DeletePantry")] //              NOAS NYA
        public async Task<IActionResult> DeletePantryAsync(int userId, string pantryName)
        {
            try
            {
                var pantry = await _pantryRepository.DeletePantryAsync(userId, pantryName);

                if (pantry == null)
                {
                    // Om pantry är null betyder det att skafferiet inte kunde hittas för borttagning
                    throw new Exception($"Pantry '{pantryName}' not found for user {userId}.");
                }

                return Ok($"Pantry '{pantryName}' deleted successfully.");
            }
            catch (Exception ex)
            {
                // Logga felmeddelandet för att spåra felet
                Console.WriteLine($"An error occurred while deleting pantry '{pantryName}' for user {userId}: {ex.Message}");

                // Returnera ett specifikt felmeddelande till användaren
                return StatusCode(500, "An error occurred while processing the request. Please try again later.");
            }
        }


        //[HttpDelete("DeletePantry")]      NOAS UTKOMMENTERADE
        //public async Task<IActionResult> DeletePantryAsync(int userId, string pantryName)
        //{
        //    try
        //    {
        //        var pantries = await _pantryRepository.DeletePantryAsync(userId, pantryName);

        //        if (pantries == null)
        //        {
        //            return NotFound("not found");
        //        }

        //        return Ok($"{pantryName} deleted");

        //    }
        //    catch (Exception ex)
        //    {
        //        return NotFound(ex.Message);
        //    }
        //}


        [HttpGet("ListPantry")] // NOAS NYA
        public async Task<IActionResult> ListUsersPantriesAsync(int userId)
        {
            try
            {
                var pantries = await _pantryRepository.ListPantriesFromUserAsync(userId);

                if (pantries == null)
                {
                    // Om skafferierna är null betyder det att inga skafferi kunde hittas för användaren
                    throw new Exception($"No pantries found for user {userId}.");
                }

                return Ok(pantries);
            }
            catch (Exception ex)
            {
                // Logga felmeddelandet för att spåra felet
                Console.WriteLine($"An error occurred while listing pantries for user {userId}: {ex.Message}");

                // Returnera ett generellt felmeddelande till användaren
                return StatusCode(500, "An error occurred while processing the request. Please try again later.");
            }
        }




        //[HttpGet("ListPantry")]    NOAS UTKOMMENTERADE
        //public async Task<IActionResult> ListUsersPantriesAsync(int userId)
        //{
        //    try
        //    {
        //        var pantries = await _pantryRepository.ListPantriesFromUserAsync(userId);

        //        if (pantries == null)
        //        {
        //            return NotFound("not found");
        //        }

        //        return Ok(pantries);

        //    }
        //    catch (Exception ex)
        //    {
        //        return NotFound(ex.Message);
        //    }

        //}

        [HttpPost("AddIngredientToPantry")]
        public async Task<IActionResult> AddIngredientToPantryAsync(PantryIngredientDto dto, string ingredientName, string pantryName, int userId)
        {
            try
            {
                await _pantryRepository.AddIngredientToPantryAsync(dto, ingredientName, pantryName, userId);

                return Ok($"{ingredientName} added to {pantryName} successfully.");

            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
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
        public async Task<IEnumerable<PantryIngredientDto>> ListPantryIngredientsAsync(int userId, string pantryName)
        {
            try
            {
                var listofPantryIngredients = await _pantryRepository.ListPantryIngredientsAsync(userId, pantryName);

                return listofPantryIngredients;
            }
            
            catch(Exception ex)
            {
                throw;
            }
        }
    }
}
