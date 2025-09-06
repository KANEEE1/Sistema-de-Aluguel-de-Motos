using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using RentalSystem.Api.Dtos;
using RentalSystem.Api.Services;
using System.Threading.Tasks;

namespace RentalSystem.Api.Controllers
{
    [ApiController]
    [Route("auth")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly ILogger<AuthController> _logger;

        public AuthController(IAuthService authService, ILogger<AuthController> logger)
        {
            _authService = authService;
            _logger = logger;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var user = await _authService.LoginAsync(loginDto);
            if (user == null)
                return Unauthorized(new { message = "Username ou senha inválidos" });

            return Ok(new
            {
                message = "Login realizado com sucesso",
                user = new
                {
                    id = user.Id,
                    username = user.Username,
                    role = user.Role
                }
            });
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] CreateUserDto createUserDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var user = await _authService.CreateUserAsync(createUserDto);
                return CreatedAtAction(nameof(Login), new { id = user.Id }, new
                {
                    message = "Usuário criado com sucesso",
                    user = new
                    {
                        id = user.Id,
                        username = user.Username,
                        role = user.Role
                    }
                });
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning("Failed to create user: {Error}", ex.Message);
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}
