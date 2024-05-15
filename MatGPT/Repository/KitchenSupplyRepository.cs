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
            try
            {
                var user = await GetKitchenSupplyFromUserAsync(userId);

                if (user == null)
                {
                    throw new Exception("User not found");
                }

                var existingKitchenSupply = user.KitchenSupplies
                    .FirstOrDefault(ks => ks.KitchenSupplyName == kitchenSupplyName);

                // Om köksredskapet inte finns, lägg till det
                if (existingKitchenSupply == null)
                {
                    user.KitchenSupplies.Add(new KitchenSupply { KitchenSupplyName = kitchenSupplyName });
                    await _context.SaveChangesAsync();
                    return "Added Kitchen Supply";
                }
                // Om köksredskapet redan finns, ta bort det
                else
                {
                    user.KitchenSupplies.Remove(existingKitchenSupply);
                    await _context.SaveChangesAsync();
                    return "Removed Kitchen Supply";
                }
            }
            catch (Exception ex)
            {
                // Logga felmeddelandet för att spåra fel
                Console.WriteLine($"An error occurred: {ex.Message}");

                // Returnera ett specifikt felmeddelande till användaren
                return "An error occurred while processing the request. Please try again later.";
            }
        }


        // NOAS KOMMENTAR
        //public async Task<string> AddOrRemoveKitchenSupplyAsync(int userId, string kitchenSupplyName)
        //{
        //    var user = await GetKitchenSupplyFromUserAsync(userId);

        //    if (user == null)
        //    {
        //        throw new Exception("User not found");
        //    }

        //    var existingKitchenSupply = user.KitchenSupplies
        //        .FirstOrDefault(ks => ks.KitchenSupplyName == kitchenSupplyName);

        //    //If kitchen supply does not exist for user, it will be added
        //    if (existingKitchenSupply == null)
        //    {
        //        user.KitchenSupplies.Add(new KitchenSupply { KitchenSupplyName = kitchenSupplyName });
        //        await _context.SaveChangesAsync();
        //        return "Added Kitchen Supply";
        //    }
        //    //If kitchen tool already exists - it will be removed
        //    else
        //    {
        //        user.KitchenSupplies.Remove(existingKitchenSupply);
        //        await _context.SaveChangesAsync();
        //        return "Removed Kitchen Supply";
        //    }
        //}


        public async Task<User> GetKitchenSupplyFromUserAsync(int userId)
        {
            var user = await _context.Users
                .Include(u => u.KitchenSupplies)
                .FirstOrDefaultAsync(u => u.UserId == userId);

            if (user == null)
            {
                throw new Exception($"User with ID {userId} not found.");
            }

            return user;
        }

        // Method for listing supplies
        public async Task<IEnumerable<KitchenSupplyViewModel>> ListKitchenSupplyFromUserAsync(int userId)
        {
            try
            {
                var user = await GetKitchenSupplyFromUserAsync(userId);

                var kitchenSuppliesViewModel = user.KitchenSupplies
                    .Select(ks => new KitchenSupplyViewModel
                    {
                        KitchenSupplyName = ks.KitchenSupplyName
                    });

                return kitchenSuppliesViewModel;
            }
            catch (Exception ex)
            {
                // Log the error for tracking
                Console.WriteLine($"An error occurred while listing kitchen supplies for user {userId}: {ex.Message}");

                // Return a generic error message to the user
                throw new Exception("An error occurred while listing kitchen supplies. Please try again later.");
            }
        }


        //// NOAS KOMMENTAR
        //public async Task<User> GetKitchenSupplyFromUserAsync(int userId)
        //{
        //    return await _context.Users
        //        .Include(u => u.KitchenSupplies)
        //        .FirstOrDefaultAsync(u => u.UserId == userId);
        //}

        //// Method for listing supplies
        //public async Task<IEnumerable<KitchenSupplyViewModel>> ListKitchenSupplyFromUserAsync(int userId)
        //{
        //    var user = await _context.Users
        //        .Include(u => u.KitchenSupplies)
        //        .FirstOrDefaultAsync(u => u.UserId == userId);

        //    if (user == null)
        //    {
        //        throw new Exception("User not found");
        //    }

        //    var kitchenSuppliesViewModel = user.KitchenSupplies
        //        .Select(ks => new KitchenSupplyViewModel
        //        {
        //            KitchenSupplyName = ks.KitchenSupplyName
        //        });

        //    return kitchenSuppliesViewModel;
        //}
    }
}
