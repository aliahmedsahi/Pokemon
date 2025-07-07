using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Moq;
using Moq.Protected;
using PokemonWebAPI.Controllers;
using PokemonWebAPI.Data;
using PokemonWebAPI.Data.Entities;
using PokemonWebAPI.Models;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace PokemonWebAPI.Tests
{
    public class PokemonControllerTests
    {
        private readonly AppDbContext _dbContext;
        private readonly PokemonController _controller;
        private readonly Mock<HttpMessageHandler> _handlerMock;

        public PokemonControllerTests()
        {
            // In-memory DB setup
            var dbOptions = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: "PokemonTestDb_Bulbasaur")
                .Options;
            _dbContext = new AppDbContext(dbOptions);

            // Mock IOptions<PokemonApiSettings>
            var optionsMock = new Mock<IOptions<PokemonApiSettings>>();
            optionsMock.Setup(o => o.Value).Returns(new PokemonApiSettings
            {
                BaseUrl = "https://pokeapi.co/api/v2"
            });

            // Mock HttpMessageHandler
            _handlerMock = new Mock<HttpMessageHandler>(MockBehavior.Strict);
            _handlerMock
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.Is<HttpRequestMessage>(req =>
                        req.Method == HttpMethod.Get &&
                        req.RequestUri.ToString().Contains("/pokemon/bulbasaur")
                    ),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(@"{
                        ""id"": 1,
                        ""name"": ""bulbasaur"",
                        ""height"": 7,
                        ""weight"": 69,
                        ""sprites"": { ""front_default"": ""https://img.pokemondb.net/sprites/bulbasaur.png"" },
                        ""abilities"": [ { ""ability"": { ""name"": ""overgrow"" } } ],
                        ""types"": [ { ""type"": { ""name"": ""grass"" } }, { ""type"": { ""name"": ""poison"" } } ]
                    }")
                })
                .Verifiable();

            var httpClient = new HttpClient(_handlerMock.Object);

            // Create controller
            _controller = new PokemonController(_dbContext, optionsMock.Object);

            // Inject HttpClient via reflection (due to private field)
            typeof(PokemonController)
                .GetField("_httpClient", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                ?.SetValue(_controller, httpClient);
        }

        [Fact]
        public async Task GetPokemon_ReturnsOk_WithValidName()
        {
            // Act
            var result = await _controller.GetPokemon("bulbasaur");

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var dto = Assert.IsType<PokemonDto>(okResult.Value);
            Assert.Equal("bulbasaur", dto.Name);
            Assert.Equal(1, dto.Id);
            Assert.Contains("grass", dto.Types);

            // Ensure SendAsync was called
            _handlerMock.Protected().Verify(
                "SendAsync",
                Times.Once(),
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>()
            );
        }
    }
}
