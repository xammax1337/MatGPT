using MatGPT.Data;
using MatGPT.Interfaces;
using MatGPT.Repository;
using Microsoft.AspNetCore.Mvc;

namespace MatGPT.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class FoodPreferenceController : Controller
    {
        private readonly ApplicationContext _context;

        private readonly IFoodPreferenceRepository _foodPreferenceRepository;

        public FoodPreferenceController(ApplicationContext dbContext, IFoodPreferenceRepository foodPreferenceRepository)
        {
            _foodPreferenceRepository = foodPreferenceRepository;
            _context = dbContext;
        }

        //Endpoint that will be used on the pages that will allow user to choose diet and allergies
        [HttpPost("FoodPreference")]
        public async Task<IActionResult> AddOrRemoveFoodPreferenceAsync(int userId, string foodPreferenceName)
        {
            try
            {
                string result = await _foodPreferenceRepository.AddOrRemoveFoodPreferenceAsync(userId, foodPreferenceName);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpGet("ListFoodPreference")]
        public async Task<IActionResult> ListFoodPreferenceFromUserAsync(int userId)
        {
            try
            {
                var foodPreferences = await _foodPreferenceRepository.ListFoodPreferenceFromUserAsync(userId);

                if (foodPreferences == null)
                {
                    return NotFound("not found");
                }

                return Ok(foodPreferences);


            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }
        }
    }
}
