using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.DTOs.AuthDTOs;
using Domain.Entities;

namespace Application.Mapper
{
    public static class AuthMapper
    {
        public static ApplicationUser ToApplicationUserFromRegisterDTO(RegisterUserDTO dto)
        {
            return new ApplicationUser
            {
                UserName = dto.Name,
                Email = dto.Email
            };
        }

        public static ApplicationUser ToApplicationUserFromLoginDTO(LoginDTO loginDTO)
        {
            return new ApplicationUser
            {
                Email = loginDTO.Email
            };
        }
    }
}
