using Domain.Entities;
using Domain.Results;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Application.DTOs.AuthDTOs
{
    public class AuthFactoryResponse 
    {
        public required string FactoryId { get; set; }
        public bool IsAuthenticated { get; set; }
        public string? FactoryName { get; set; }
        public string? Email { get; set; }
        public string? Token { get; set; }
        public DateTime? ExpiresOn { get; set; }
        [JsonIgnore]
        public string? RefreshToken { get; set; }
        public DateTime RefreshTokenExpiration { get; set; }




    }
}
