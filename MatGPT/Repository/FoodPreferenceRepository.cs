using MatGPT.Data;
using MatGPT.Interfaces;
using MatGPT.Models;
using MatGPT.Models.ViewModels;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace MatGPT.Repository
{
    public class FoodPreferenceRepository : IFoodPreferenceRepository
    {
        private readonly ApplicationContext _context;
        public FoodPreferenceRepository(ApplicationContext context)
        {
            _context = context;
        }

        public async Task<bool> UserExistsAsync(int userId)
        {
            var user = await _context.Users.FindAsync(userId);
            return user != null;
        }

        public async Task<string> AddOrRemoveFoodPreferenceAsync(int userId, string foodPreferenceName)
        {
            try
            {

                var userExists = await UserExistsAsync(userId);
                if (!userExists)
                {
                    return "User not found";
                }

                var user = await GetFoodPreferenceFromUserAsync(userId);

   
                //if (user == null)
                //{
                //    throw new Exception("User not found");
                //}

                var existingfoodPreference = user.FoodPreferences
                    .FirstOrDefault(ks => ks.FoodPreferenceName == foodPreferenceName);

                //If food preference does not exist for user, it will be added
                if (existingfoodPreference == null)
                {
                    user.FoodPreferences.Add(new FoodPreference { FoodPreferenceName = foodPreferenceName });
                    await _context.SaveChangesAsync();
                    return "Added Food Preference";
                }
                //If food preference already exists - it will be removed
                else
                {
                    user.FoodPreferences.Remove(existingfoodPreference);
                    await _context.SaveChangesAsync();
                    return "Removed Food Preference";
                }
            }
            catch (Exception ex)
            {
                return "Error when handling request";

            }
        }

        //Fetches food preferences and sends them to GenerateRecipeAsync
        public async Task<User> GetFoodPreferenceFromUserAsync(int userId)
        {
            var user = await _context.Users
                .Include(u => u.FoodPreferences)
                .FirstOrDefaultAsync(u => u.UserId == userId);

            var userExists = await UserExistsAsync(userId);
            if (!userExists)
            {
                throw new Exception("User not found");
            }

            //if (user == null)
            //{
            //    throw new Exception("User not found");
            //}

            return user;
        }

        //Finds the logged in user's food preferences (if any have been chosen) and lists them
        public async Task<IEnumerable<FoodPreferenceViewModel>> ListFoodPreferenceFromUserAsync(int userId)
        {
            try
            {
                var userExists = await UserExistsAsync(userId);
                if (!userExists)
                {
                    throw new Exception("User not found");
                }


                var user = await _context.Users
                    .Include(u => u.FoodPreferences)
                    .FirstOrDefaultAsync(u => u.UserId == userId);

                //if (user == null)
                //{
                //    throw new Exception("User not found");
                //}

                var foodPreferenceViewModel = user.FoodPreferences
                    .Select(fp => new FoodPreferenceViewModel
                    {
                        FoodPreferenceName = fp.FoodPreferenceName
                    });

                return foodPreferenceViewModel;
            }
            catch (Exception ex)
            {
                throw new Exception("Error when handling request");
            }
        }
    }
}