using MatGPT.Models;
using MatGPT.Models.ViewModels;

namespace MatGPT.Interfaces
{
    public interface IKitchenSupplyRepository
    {
        Task<User> GetKitchenSupplyFromUserAsync(int userId);
        Task<string> AddOrRemoveKitchenSupplyAsync(int userId, string kitchenSupplyName);
        Task<IEnumerable<KitchenSupplyViewModel>> ListKitchenSupplyFromUserAsync(int userId);
    }
}
