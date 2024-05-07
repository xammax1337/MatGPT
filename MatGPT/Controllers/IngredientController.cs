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
            try
            {
                var ingredient = await _ingredientRepository.AddIngredientAsync(dto, ingredientName, userId);

                if (ingredient == null)
                {
                    return NotFound("not found");
                }

                return Ok($"Ingredient {ingredientName} added successfully.");

            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }
        }

        //In order to use the DELETEs: We have to use the GET that lists the items first,
        //then we use the DELETE
        [HttpDelete("DeleteIngredient")]
        public async Task<IActionResult> DeleteIngredientAsync(int userId, string ingredientName)
        {
            try
            {
                var ingredients = await _ingredientRepository.DeleteIngredientAsync(userId, ingredientName);

                if (ingredients == null)
                {
                    return NotFound("not found");
                }

                return Ok($"{ingredientName} deleted");

            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpGet("ListIngredients")]
        public async Task<IActionResult> ListUsersIngredientsAsync(int userId)
        {
            try
            {
                var ingredients = await _ingredientRepository.ListIngredientsFromUserAsync(userId);

                if (ingredients == null)
                {
                    return NotFound("not found");
                }

                return Ok(ingredients);

            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }
        }
    }
}
