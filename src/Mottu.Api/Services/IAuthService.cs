using Mottu.Api.Dtos;
using Mottu.Api.Entities;

namespace Mottu.Api.Services
{
    public interface IAuthService
    {
        Task<User?> LoginAsync(LoginDto loginDto);
        Task<User> CreateUserAsync(CreateUserDto createUserDto);
        Task<bool> IsAdminAsync(string username);
        Task<bool> IsDeliveryPersonAsync(string username);
    }
}
