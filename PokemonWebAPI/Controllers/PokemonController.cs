namespace PokemonWebAPI.Controllers
{
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Options;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;
    using PokemonWebAPI.Data;
    using PokemonWebAPI.Data.Entities;
    using PokemonWebAPI.Models;

    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class PokemonController : ControllerBase
    {
        private readonly HttpClient _httpClient;
        private readonly AppDbContext _context;
        private readonly string _pokemonApiBaseUrl;

        public PokemonController(AppDbContext context, IOptions<PokemonApiSettings> options)
        {
            _httpClient = new HttpClient();
            _context = context;
            _pokemonApiBaseUrl = options.Value.BaseUrl;
        }

        /// <summary>
        /// Gets a Pokémon by name or ID from the external API and saves it to the local database.
        /// </summary>
        /// <param name="nameOrId">Pokémon name or ID.</param>
        /// <returns>Pokémon data transfer object.</returns>
        [HttpGet("{nameOrId}")]
        public async Task<IActionResult> GetPokemon(string nameOrId)
        {
            var pokemon = await GetPokemonFromApiAsync(nameOrId);

            if (pokemon == null)
                return NotFound(new { Error = $"Pokémon '{nameOrId}' not found." });

            await SavePokemonAsync(pokemon);

            var dto = MapToDto(pokemon);

            return Ok(dto);
        }

        /// <summary>
        /// Gets all Pokémon stored in the local database.
        /// </summary>
        /// <returns>List of Pokémon data transfer objects.</returns>
        [AllowAnonymous]
        [HttpGet("get-all-pokemon")]
        public async Task<IActionResult> GetAll()
        {
            var pokemons = await _context.Pokemons
                .OrderByDescending(p => p.CreatedAt)
                .ToListAsync();

            var dtoList = pokemons.Select(MapToDto).ToList();

            return Ok(dtoList);
        }

        /// <summary>
        /// Maps a Pokémon entity to its DTO representation.
        /// </summary>
        private static PokemonDto MapToDto(Pokemon pokemon) => new()
        {
            Id = pokemon.PokeApiId,
            Name = pokemon.Name,
            Sprite = pokemon.SpriteUrl,
            Height = pokemon.Height,
            Weight = pokemon.Weight,
            Abilities = pokemon.Abilities,
            Types = pokemon.Types
        };

        /// <summary>
        /// Retrieves a Pokémon from the external API.
        /// </summary>
        private async Task<Pokemon?> GetPokemonFromApiAsync(string nameOrId)
        {
            var url = $"{_pokemonApiBaseUrl}/pokemon/{nameOrId.ToLower()}";

            HttpResponseMessage response;
            try
            {
                response = await _httpClient.GetAsync(url);
            }
            catch
            {
                return null;
            }

            if (!response.IsSuccessStatusCode)
                return null;

            var content = await response.Content.ReadAsStringAsync();
            var json = JsonConvert.DeserializeObject<JObject>(content);
            if (json == null) return null;

            return new Pokemon
            {
                PokeApiId = json["id"]?.Value<int>() ?? 0,
                Name = json["name"]?.Value<string>() ?? "Unknown",
                SpriteUrl = json["sprites"]?["front_default"]?.Value<string>() ?? "",
                Height = json["height"]?.Value<int>() ?? 0,
                Weight = json["weight"]?.Value<int>() ?? 0,
                Abilities = json["abilities"]?
                    .Select(a => a["ability"]?["name"]?.Value<string>())
                    .Where(n => !string.IsNullOrWhiteSpace(n))
                    .ToList() ?? new List<string>(),
                Types = json["types"]?
                    .Select(t => t["type"]?["name"]?.Value<string>())
                    .Where(n => !string.IsNullOrWhiteSpace(n))
                    .ToList() ?? new List<string>(),
                CreatedAt = DateTime.UtcNow
            };
        }

        /// <summary>
        /// Saves a Pokémon to the local database and ensures only the 15 most recent are kept.
        /// </summary>
        private async Task SavePokemonAsync(Pokemon pokemon)
        {
            _context.Pokemons.Add(pokemon);
            await _context.SaveChangesAsync();

            var excess = await _context.Pokemons
                .OrderByDescending(p => p.CreatedAt)
                .Skip(15)
                .ToListAsync();

            if (excess.Any())
            {
                _context.Pokemons.RemoveRange(excess);
                await _context.SaveChangesAsync();
            }
        }
    }

}
