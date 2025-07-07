namespace PokemonWebAPI.Data.Entities
{
    public class Pokemon
    {
        public int Id { get; set; } // Local DB ID

        public int PokeApiId { get; set; } // ID from Pokémon API

        public string Name { get; set; }

        public int Height { get; set; } // In decimetres

        public int Weight { get; set; } // In hectograms

        public string SpriteUrl { get; set; } // Image from the API

        public List<string> Types { get; set; } = new List<string>();

        public List<string> Abilities { get; set; } = new List<string>();

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedAt { get; set; }
    }

}
