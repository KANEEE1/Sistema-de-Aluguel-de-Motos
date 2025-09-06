using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using RentalSystem.Api.Data;
using RentalSystem.Api.Dtos;
using RentalSystem.Api.Entities;
using System.Security.Cryptography;
using System.Text;

namespace RentalSystem.Api.Services
{
    public class AuthService : IAuthService
    {
        private readonly RentalSystemDbContext _context;
        private readonly ILogger<AuthService> _logger;

        public AuthService(RentalSystemDbContext context, ILogger<AuthService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<User?> LoginAsync(LoginDto loginDto)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Username == loginDto.Username && u.IsActive);

            if (user == null)
                return null;

            var hashedPassword = HashPassword(loginDto.Password);
            if (user.PasswordHash != hashedPassword)
                return null;

            _logger.LogInformation("User {Username} logged in successfully", user.Username);
            return user;
        }

        public async Task<User> CreateUserAsync(CreateUserDto createUserDto)
        {
            var existingUser = await _context.Users
                .FirstOrDefaultAsync(u => u.Username == createUserDto.Username);

            if (existingUser != null)
                throw new InvalidOperationException($"Username {createUserDto.Username} já existe");

            var user = new User
            {
                Id = Guid.NewGuid(),
                Username = createUserDto.Username,
                Email = createUserDto.Email,
                PasswordHash = HashPassword(createUserDto.Password),
                Role = createUserDto.Role,
                CreatedAt = DateTime.UtcNow,
                IsActive = true
            };

            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();

            _logger.LogInformation("User {Username} created with role {Role}", user.Username, user.Role);
            return user;
        }

        public async Task<bool> IsAdminAsync(string username)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Username == username && u.IsActive);

            return user?.Role == "Admin";
        }

        public async Task<bool> IsDeliveryPersonAsync(string username)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Username == username && u.IsActive);

            return user?.Role == "DeliveryPerson";
        }

        private string HashPassword(string password)
        {
            using var sha256 = SHA256.Create();
            var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
            return Convert.ToBase64String(hashedBytes);
        }
    }
}
