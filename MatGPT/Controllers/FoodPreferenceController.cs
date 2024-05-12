using MatGPT.Data;
using MatGPT.Interfaces;
using MatGPT.Repository;
using MatGPT.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MatGPT.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class FoodPreferenceController : Controller
    {
        private readonly UserService _userService;

        private readonly IFoodPreferenceRepository _foodPreferenceRepository;

        public FoodPreferenceController(UserService userService, IFoodPreferenceRepository foodPreferenceRepository)
        {
            _foodPreferenceRepository = foodPreferenceRepository;
            _userService = userService;
        }

        //Endpoint that will be used on the pages that will allow user to choose diet and allergies
        [HttpPost("FoodPreference")]
        [Authorize]
        public async Task<IActionResult> AddOrRemoveFoodPreferenceAsync(string foodPreferenceName)
        {
            // Gets the userId from the JWTtoken and checks if hte userId is not -1
            int userId = _userService.GetUserIdFromToken(User);
            if (userId == -1)
            {
                return Unauthorized("User ID not found in token.");
            }

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
        [Authorize]
        public async Task<IActionResult> ListFoodPreferenceFromUserAsync()
        {
            // Gets the userId from the JWTtoken and checks if hte userId is not -1
            int userId = _userService.GetUserIdFromToken(User);
            if (userId == -1)
            {
                return Unauthorized("User ID not found in token.");
            }
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
