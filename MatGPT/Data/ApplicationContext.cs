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

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<PantryFoodItem>()
                .HasOne(r => r.FoodItem)
                .WithMany(b=>b.PantryFoodItems) 
                .HasForeignKey(f => f.FoodItemId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<PantryFoodItem>()
               .HasOne(r => r.Pantry)
               .WithMany(p=>p.PantryFoodItems)
               .HasForeignKey(f => f.PantryId)
               .OnDelete(DeleteBehavior.Restrict);
        }
    }

}
