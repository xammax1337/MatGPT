using MatGPT.Models;
using Microsoft.EntityFrameworkCore;

namespace MatGPT.Data
{
    public class ApplicationContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Credential> Credentials { get; set; }
        public DbSet<FoodItem> FoodItems { get; set; }
        public DbSet<Recipe> Recipes { get; set; }
        public DbSet<FoodPreference> FoodPreferences { get; set; }
        public DbSet <KitchenSupply> KitchenSupply { get; set;}

        public ApplicationContext(DbContextOptions<ApplicationContext> options) : base(options) { }
    }
}
