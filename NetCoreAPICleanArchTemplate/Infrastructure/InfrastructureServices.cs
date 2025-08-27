using Application.Contracts;
using Infrastructure.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure
{
    public static class InfrastructureServices
    {
        public static IServiceCollection AddInfrastructureServices(this IServiceCollection services)
        {
            // Repositories and Services
            services.AddScoped(typeof(IGeneralRepoistory<>), typeof(GeneralRepoistory<>));
            services.AddScoped<IUserRepoistory, UserRepoistory>();
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IJwtTokenService, JWTTokenService>();
            services.AddScoped(typeof(IHashingService<>), typeof(HashingService<>));
            services.AddScoped<IFactoryRepoistory, FactoryRepository>();
            return services;
        }
    }
}
