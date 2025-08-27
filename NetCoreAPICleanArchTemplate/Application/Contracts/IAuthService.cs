

using Application.DTOs.AuthDTOs;
using Domain.Entities;
using Domain.Results;

namespace Application.Contracts
{
    public interface IAuthService 
    {
        Task<Response<AuthResult>> RegisterUserAsync(RegisterUserDTO dto);
        Task<Response<AuthFactoryResponse>> RegisterFactoryAsync(RegisterFactoryDTO dto);
        Task<Response<AuthResult>> LoginUserAsync(LoginDTO dto);

        Task<Response<AuthFactoryResponse>> LoginFactoryAsync(LoginDTO dto);

    }
}
