using System.Threading.Tasks;
using Application.DTOs;

namespace Application.Contracts
{
    public interface IAuthService
    {
        Task<AuthResponseDto> RegisterUserAsync(UserRegistrationDto request);
        Task<AuthResponseDto> RegisterFactoryAsync(FactoryRegistrationDto request);
        Task<AuthResponseDto> LoginAsync(LoginDto request);
        Task<bool> ValidateTokenAsync(string token);
        Task<AuthResponseDto> ChangePasswordAsync(string email, ChangePasswordDto request);
        Task<SignOutResponseDto> SignOutAsync(string token);
    }
}
