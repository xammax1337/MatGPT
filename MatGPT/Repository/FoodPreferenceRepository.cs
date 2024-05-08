using MatGPT.Data;
using MatGPT.Interfaces;
using MatGPT.Models;
using MatGPT.Models.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace MatGPT.Repository
{
    public class FoodPreferenceRepository : IFoodPreferenceRepository
    {
        private readonly ApplicationContext _context;
        public FoodPreferenceRepository(ApplicationContext context)
        {
            _context = context;
        }

        public async Task<string> AddOrRemoveFoodPreferenceAsync(int userId, string foodPreferenceName)
        {
            var user = await GetFoodPreferenceFromUserAsync(userId);

            if (user == null)
            {
                throw new Exception("User not found");
            }

            var existingfoodPreference = user.FoodPreferences
                .FirstOrDefault(ks => ks.FoodPreferenceName == foodPreferenceName);

            //If kitchen supply does not exist for user, it will be added
            if (existingfoodPreference == null)
            {
                user.FoodPreferences.Add(new FoodPreference { FoodPreferenceName = foodPreferenceName });
                await _context.SaveChangesAsync();
                return "Added Food Preference";
            }
            //If kitchen tool already exists - it will be removed
            else
            {
                user.FoodPreferences.Remove(existingfoodPreference);
                await _context.SaveChangesAsync();
                return "Removed Food Preference";
            }

        }

        public async Task<User> GetFoodPreferenceFromUserAsync(int userId)
        {
            return await _context.Users
                .Include(u => u.FoodPreferences)
                .FirstOrDefaultAsync(u => u.UserId == userId);
        }

        public async Task<IEnumerable<FoodPreferenceViewModel>> ListFoodPreferenceFromUserAsync(int userId)
        {
            var user = await _context.Users
                .Include(u => u.FoodPreferences)
                .FirstOrDefaultAsync(u => u.UserId == userId);

            if (user == null)
            {
                throw new Exception("User not found");
            }

            var foodPreferenceViewModel = user.FoodPreferences
                .Select(fp => new FoodPreferenceViewModel
                {
                    FoodPreferenceName = fp.FoodPreferenceName
                });

            return foodPreferenceViewModel;
        }
    }
}
