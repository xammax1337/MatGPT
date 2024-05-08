using MatGPT.Models;
using Microsoft.EntityFrameworkCore;

namespace MatGPT.Data
{
    public class ApplicationContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Credential> Credentials { get; set; }
        public DbSet<Ingredient> Ingredients { get; set; }
        public DbSet<Recipe> Recipes { get; set; }
        public DbSet<FoodPreference> FoodPreferences { get; set; }
        public DbSet <KitchenSupply> KitchenSupplies { get; set;}

        public DbSet<Pantry>Pantries { get; set;}
        public DbSet<PantryIngredient> PantryIngredients {  get; set; }
        public ApplicationContext(DbContextOptions<ApplicationContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<PantryIngredient>()
                .HasOne(r => r.Ingredient)
                .WithMany(b=>b.PantryIngredients) 
                .HasForeignKey(f => f.IngredientId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<PantryIngredient>()
               .HasOne(r => r.Pantry)
               .WithMany(p=>p.PantryIngredients)
               .HasForeignKey(f => f.PantryId)
               .OnDelete(DeleteBehavior.Restrict);
        }
    }

}
