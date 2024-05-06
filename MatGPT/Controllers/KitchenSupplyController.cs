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
                string result = await _kitchenSupplyRepository.AddOrRemoveKitchenSupplyAsync(userId, kitchenSupplyName);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpGet("ListKitchenSupply")]
        public async Task<IActionResult> GetKitchenSupplyFromUserAsync(int userId)
        {
            try
            {
                var kitchenSupplies = await _kitchenSupplyRepository.ListKitchenSupplyFromUserAsync(userId);

                if (kitchenSupplies == null)
                {
                    return NotFound("not found");
                }

                return Ok(kitchenSupplies);


            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }
        }
    }
}
