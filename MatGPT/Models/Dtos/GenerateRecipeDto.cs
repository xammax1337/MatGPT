namespace MatGPT.Models.Dtos
{
    public class GenerateRecipeDto
    {
        public string Query { get; set; }
        public int UserId { get; set; }
        public int MinTime { get; set; }
        public int MaxTime { get; set; }
        public bool ChooseTimer { get; set; }
        public int Servings { get; set; }
        public bool ChoosePreferences { get; set; }
    }
}
