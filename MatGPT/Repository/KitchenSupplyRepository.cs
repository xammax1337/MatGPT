using MatGPT.Data;
using MatGPT.Interfaces;
using MatGPT.Models;
using MatGPT.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MatGPT.Repository
{
    public class KitchenSupplyRepository : IKitchenSupplyRepository
    {
        private readonly ApplicationContext _context;
        public KitchenSupplyRepository(ApplicationContext context)
        {
            _context = context;
        }

        public async Task<string> AddOrRemoveKitchenSupplyAsync(int userId, string kitchenSupplyName)
        {
            var user = await GetKitchenSupplyFromUserAsync(userId);

            if (user == null)
            {
                throw new Exception("User not found");
            }

            var existingKitchenSupply = user.KitchenSupplies
                .FirstOrDefault(ks => ks.KitchenSupplyName == kitchenSupplyName);

            //If kitchen supply does not exist for user, it will be added
            if (existingKitchenSupply == null)
            {
                user.KitchenSupplies.Add(new KitchenSupply { KitchenSupplyName = kitchenSupplyName });
                await _context.SaveChangesAsync();
                return "Added Kitchen Supply";
            }
            //If kitchen tool already exists - it will be removed
            else
            {
                user.KitchenSupplies.Remove(existingKitchenSupply);
                await _context.SaveChangesAsync();
                return "Removed Kitchen Supply";
            }
        }

        public async Task<User> GetKitchenSupplyFromUserAsync(int userId)
        {
            return await _context.Users
                .Include(u => u.KitchenSupplies)
                .FirstOrDefaultAsync(u => u.UserId == userId);
        }

        // Method for listing supplies
        public async Task<IEnumerable<KitchenSupplyViewModel>> ListKitchenSupplyFromUserAsync(int userId)
        {
            var user = await _context.Users
                .Include(u => u.KitchenSupplies)
                .FirstOrDefaultAsync(u => u.UserId == userId);

            if (user == null)
            {
                throw new Exception("User not found");
            }

            var kitchenSuppliesViewModel = user.KitchenSupplies
                .Select(ks => new KitchenSupplyViewModel
                {
                    KitchenSupplyName = ks.KitchenSupplyName
                });

            return kitchenSuppliesViewModel;
        }
    }
}
