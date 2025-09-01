using System.Threading.Tasks;
using Application.DTOs;
using Domain.Results;

namespace Application.Contracts
{
    public interface IAuthService
    {
        Task<AuthResponseDto> RegisterUserAsync(UserRegistrationDto request);
        Task<AuthResponseDto> RegisterFactoryAsync(FactoryRegistrationDto request);
        Task<AuthResponseDto> LoginAsync(LoginDto request);
        Task<bool> ValidateTokenAsync(string token);
    }
}
