using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using PokemonWebAPI.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Text.RegularExpressions;


namespace PokemonWebAPI.Controllers
{

    /// <summary>
    /// Handles authentication-related endpoints such as registration and login.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IConfiguration _config;

        public AuthController(UserManager<ApplicationUser> userManager, IConfiguration config)
        {
            _userManager = userManager;
            _config = config;
        }

        /// <summary>
        /// Registers a new user with the provided email and password.
        /// </summary>
        /// <param name="dto">Registration data transfer object.</param>
        /// <returns>Action result indicating success or failure.</returns>
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto dto)
        {
            if (!IsValidEmail(dto.Email))
            {
                return BadRequest(new { Error = "Please enter a valid email address format." });
            }

            var user = new ApplicationUser { UserName = dto.Email, Email = dto.Email };
            var result = await _userManager.CreateAsync(user, dto.Password);

            if (!result.Succeeded)
            {
                var errors = result.Errors.Select(e => e.Description).ToArray();
                return BadRequest(new { Errors = errors });
            }

            return Ok(new { Message = "Registration successful." });
        }

        /// <summary>
        /// Authenticates a user and returns a JWT token if successful.
        /// </summary>
        /// <param name="dto">Login data transfer object.</param>
        /// <returns>JWT token or unauthorized result.</returns>
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto dto)
        {
            var user = await _userManager.FindByEmailAsync(dto.Email);
            if (user == null || !await _userManager.CheckPasswordAsync(user, dto.Password))
            {
                return Unauthorized(new { Error = "Invalid credentials." });
            }

            var token = GenerateJwtToken(user);
            return Ok(new { Token = token });
        }

        /// <summary>
        /// Generates a JWT token for the specified user.
        /// </summary>
        /// <param name="user">The application user.</param>
        /// <returns>JWT token string.</returns>
        private string GenerateJwtToken(ApplicationUser user)
        {
            var claims = new List<Claim>
                {
                    new(JwtRegisteredClaimNames.Sub, user.Id ?? string.Empty),
                    new(JwtRegisteredClaimNames.Email, user.Email ?? string.Empty)
                };

            var secretKey = _config["JwtSettings:SecretKey"];
            if (string.IsNullOrWhiteSpace(secretKey))
            {
                throw new InvalidOperationException("JWT SecretKey is not configured.");
            }

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.UtcNow.AddHours(1),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        /// <summary>
        /// Validates the email format using a regular expression.
        /// </summary>
        /// <param name="email">Email address to validate.</param>
        /// <returns>True if valid, otherwise false.</returns>
        private static bool IsValidEmail(string email)
        {
            const string emailPattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
            return !string.IsNullOrWhiteSpace(email) &&
                   Regex.IsMatch(email, emailPattern, RegexOptions.IgnoreCase);
        }
    }

}
