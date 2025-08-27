using Application.Contracts;
using Domain.Entities;
using Domain.enums;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Services
{
	public class JWTTokenService : IJwtTokenService
	{
		private readonly IConfiguration _configuration;
		private readonly string _key;
		private readonly string _issuer;
		private readonly string _audience;
		private readonly int _expirationHours;


		public JWTTokenService(IConfiguration configuration)
		{
			_configuration = configuration;
			_key = _configuration["JwtSettings:Key"] ?? throw new InvalidOperationException("JWT Key not found in configuration");
			_issuer = _configuration["JwtSettings:Issuer"] ?? throw new InvalidOperationException("JWT Issuer not found in configuration");
			_audience = _configuration["JwtSettings:Audience"] ?? throw new InvalidOperationException("JWT Audience not found in configuration");
			_expirationHours = int.Parse(_configuration["JwtSettings:ExpirationHours"] ?? "24");

		}
		public Task<string> GenerateToken(ApplicationUser user)
		{
			var tokenHandler = new JwtSecurityTokenHandler();
			var key = Encoding.ASCII.GetBytes(_key);

			var claims = new List<Claim>
			{
				new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
				new Claim(ClaimTypes.Email, user.Email),
				new Claim(ClaimTypes.Role, user.Role.ToString()),
				new Claim("UserId" , user.Id.ToString()),
				new Claim("UserRole" , user.Role.ToString())
			};

			var tokenDescriptor = new SecurityTokenDescriptor
			{
				Subject = new ClaimsIdentity(claims),
				Expires = DateTime.UtcNow.AddHours(_expirationHours),
				Issuer = _issuer,
				Audience = _audience,
				SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
			};

			var token = tokenHandler.CreateToken(tokenDescriptor);
			return Task.FromResult(tokenHandler.WriteToken(token));


		}

		public async Task<ClaimsPrincipal> ValidateToken(string token)
		{
			if (string.IsNullOrEmpty(token)) return null;

			var tokenHandler = new JwtSecurityTokenHandler();
			var key = Encoding.UTF8.GetBytes(_key);

			try
			{
				var validationParameters = new TokenValidationParameters
				{
					ValidateIssuer = true,
					ValidateAudience = true,
					ValidateLifetime = true,
					ValidateIssuerSigningKey = true,
					ValidIssuer = _issuer,
					ValidAudience = _audience,
					IssuerSigningKey = new SymmetricSecurityKey(key),
					ClockSkew = TimeSpan.Zero
				};

				var principal = tokenHandler.ValidateToken(token, validationParameters, out var validatedToken);
				return principal;
			}
			catch (SecurityTokenExpiredException)
			{
				return null;
			}
			catch (SecurityTokenInvalidSignatureException)
			{
				return null;
			}
			catch (Exception)
			{
				return null;
			}
		}

		public Task<string> GenerateFactoryToken(Factory factory)
		{
			var tokenHandler = new JwtSecurityTokenHandler();
			var key = Encoding.ASCII.GetBytes(_key);

			var claims = new List<Claim>
		{
			new Claim(ClaimTypes.NameIdentifier, factory.Id.ToString()),
			new Claim(ClaimTypes.Email, factory.Email),
			new Claim("FactoryName", factory.FactoryName)
		};

			var tokenDescriptor = new SecurityTokenDescriptor
			{
				Subject = new ClaimsIdentity(claims),
				Expires = DateTime.UtcNow.AddHours(_expirationHours),
				Issuer = _issuer,
				Audience = _audience,
				SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
			};

			var token = tokenHandler.CreateToken(tokenDescriptor);
			return Task.FromResult(tokenHandler.WriteToken(token));
		}
	}
}