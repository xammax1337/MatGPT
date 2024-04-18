namespace MatGPT.Models
{
    public class FoodPreference
    {
        public int FoodPreferenceId { get; set; }
        public string FoodPreferenceName { get; set; }
        public int UserId { get; set; }
        public User User { get; set; }


    }
}
