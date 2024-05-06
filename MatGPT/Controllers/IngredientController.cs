using MatGPT.Data;
using MatGPT.Interfaces;
using MatGPT.Models.Dtos;
using MatGPT.Models.ViewModels;
using MatGPT.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OpenAI_API;
using MatGPT.Repository;

namespace MatGPT.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class IngredientController : Controller
    {
        private readonly OpenAIAPI _api;
        private readonly IIngredientRepository _ingredientRepository;

        public IngredientController(OpenAIAPI api, IIngredientRepository ingredientRepository)
        {
            _api = api;
            _ingredientRepository = ingredientRepository;
        }

        [HttpPost("AddIngredient")]
        public async Task<IActionResult> AddIngredientAsync(IngredientDto dto, string ingredientName, int userId)
        {
            await _ingredientRepository.AddIngredientAsync(dto, ingredientName, userId);
            return Ok($"Ingredient {ingredientName} added successfully.");
        }

        //In order to use the DELETEs: We have to use the GET that lists the items first,
        //then we use the DELETE
        [HttpDelete("DeleteIngredient")]
        public async Task<IActionResult> DeleteIngredientAsync(int userId, string ingredientName)
        {
            await _ingredientRepository.DeleteIngredientAsync(userId, ingredientName);
            return Ok($"{ingredientName} deleted");
        }

        [HttpGet("ListIngredients")]
        public async Task<IActionResult> ListUsersIngredientsAsync(int userId)
        {
            try
            {
                var pantries = await _ingredientRepository.ListIngredientsFromUserAsync(userId);

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
    }
}
