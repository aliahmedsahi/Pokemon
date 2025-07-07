namespace PokemonWebAPI.Models
{
    public class PokemonDto
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Sprite { get; set; }
        public int Height { get; set; }
        public int Weight { get; set; }
        public List<string>? Abilities { get; set; }
        public List<string>? Types { get; set; }
    }

}
