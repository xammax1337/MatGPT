using MatGPT.Data;

namespace MatGPT.Repositories
{
    public interface IRecipeRepository
    {

    }

    public class RecipeRepository : IRecipeRepository
    {
        private readonly ApplicationContext _context;
        public RecipeRepository(ApplicationContext context) 
        {
            _context = context;
        }
    }
}
