using RentalSystem.Api.Dtos;
using RentalSystem.Api.Entities;

namespace RentalSystem.Api.Services
{
    public interface IAuthService
    {
        Task<User?> LoginAsync(LoginDto loginDto);
        Task<User> CreateUserAsync(CreateUserDto createUserDto);
        Task<bool> IsAdminAsync(string username);
        Task<bool> IsDeliveryPersonAsync(string username);
    }
}
