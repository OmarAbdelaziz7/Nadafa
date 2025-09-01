using Application.Contracts;
using Application.Implementations;
using Microsoft.Extensions.DependencyInjection;


namespace Application
{
    public static class ApplicationServices
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            services.AddScoped<IAuthService, AuthService>();
            //services.AddScoped<IUnitOfWork, UnitOfWork>();
            //services.AddScoped<ISP_Call, SP_Call>();
            //services.AddScoped<IEmailService, EmailService>();

            return services;
        }
    }
}
