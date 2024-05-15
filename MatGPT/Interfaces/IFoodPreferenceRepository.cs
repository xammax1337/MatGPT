using MatGPT.Models;
using MatGPT.Models.ViewModels;

namespace MatGPT.Interfaces
{
    public interface IFoodPreferenceRepository
    {
        Task<User> GetFoodPreferenceFromUserAsync(int userId);
        Task<string> AddOrRemoveFoodPreferenceAsync(int userId, string foodPreferenceName);
        Task<IEnumerable<FoodPreferenceViewModel>> ListFoodPreferenceFromUserAsync(int userId);
        Task<bool> UserExistsAsync(int userId);
    }
}