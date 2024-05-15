using MatGPT.Data;
using MatGPT.Interfaces;
using MatGPT.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;
using System.Text.Json;

namespace MatGPT.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class KitchenSupplyController : Controller
    {
        private readonly ApplicationContext _context;

        private readonly IKitchenSupplyRepository _kitchenSupplyRepository;

        public KitchenSupplyController(ApplicationContext dbContext, IKitchenSupplyRepository kitchenSupplyRepository)
        {
            _kitchenSupplyRepository = kitchenSupplyRepository;
            _context = dbContext;
        }

        //EndPoint that will be used on the page that will allow user to choose available kitchen tools

        [HttpPost("KitchenSupply")]
        public async Task<IActionResult> AddOrRemoveKitchenSupplyAsync(int userId, string kitchenSupplyName)
        {
            try
            {
                // Kontrollera om användar-ID finns
                var user = await _kitchenSupplyRepository.GetKitchenSupplyFromUserAsync(userId);
                if (user == null)
                {
                    return NotFound($"User with ID {userId} not found.");
                }

                // Kontrollera om supply finns
                if (string.IsNullOrEmpty(kitchenSupplyName))
                {
                    return BadRequest("Please enter a kitchen supply.");
                }

                // Fortsätt med att lägga till eller ta bort köksredskapet
                string result = await _kitchenSupplyRepository.AddOrRemoveKitchenSupplyAsync(userId, kitchenSupplyName);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }

        //[HttpPost("KitchenSupply")] NOAS UTKOMMENTERADE
        //public async Task<IActionResult> AddOrRemoveKitchenSupplyAsync(int userId, string kitchenSupplyName)
        //{
        //    try
        //    {
        //        string result = await _kitchenSupplyRepository.AddOrRemoveKitchenSupplyAsync(userId, kitchenSupplyName);
        //        return Ok(result);
        //    }
        //    catch (Exception ex)
        //    {
        //        return NotFound(ex.Message);
        //    }
        //}


        [HttpGet("ListKitchenSupply")]
        public async Task<IActionResult> GetKitchenSupplyFromUserAsync(int userId)
        {
            try
            {
                var kitchenSupplies = await _kitchenSupplyRepository.ListKitchenSupplyFromUserAsync(userId);

                if (kitchenSupplies == null)
                {
                    return NotFound("Kitchen supplies not found for the specified user.");
                }

                return Ok(kitchenSupplies);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }


        //[HttpGet("ListKitchenSupply")] NOAS UTKOMMENTERADE
        //public async Task<IActionResult> GetKitchenSupplyFromUserAsync(int userId)
        //{
        //    try
        //    {
        //        var kitchenSupplies = await _kitchenSupplyRepository.ListKitchenSupplyFromUserAsync(userId);

        //        if (kitchenSupplies == null)
        //        {
        //            return NotFound("not found");
        //        }

        //        return Ok(kitchenSupplies);


        //    }
        //    catch (Exception ex)
        //    {
        //        return NotFound(ex.Message);
        //    }
        //}
    }
}
