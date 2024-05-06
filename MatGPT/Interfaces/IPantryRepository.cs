using MatGPT.Models;
using MatGPT.Models.Dtos;
using MatGPT.Models.ViewModels;

namespace MatGPT.Interfaces
{
    public interface IPantryRepository
    {
        Task<Pantry> AddPantryAsync(PantryDto dto, string pantryName, int userId);
        Task<Pantry> DeletePantryAsync(int userId, string pantryName);
        Task<IEnumerable<PantryViewModel>> ListPantriesFromUserAsync(int userId);

    }
}
