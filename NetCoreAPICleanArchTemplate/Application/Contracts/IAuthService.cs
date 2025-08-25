

using Application.DTOs.AuthDTOs;
using Domain.Entities;
using Domain.Results;

namespace Application.Contracts
{
    public interface IAuthService 
    {
        Task<Response<AuthResult>> RegisterUserAsync(RegisterUserDTO dto);
        /*Task<Response<AuthResult>> RegisterFactoryAsync(RegisterFactoryDTO dto);*/
        Task<Response<AuthResult>> LoginAsync(LoginDTO dto);

    }
}
