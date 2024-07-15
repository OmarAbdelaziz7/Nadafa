using Microsoft.Extensions.DependencyInjection;


namespace Application
{
    public static class ApplicationServices
    {
        public static IServiceCollection AddDomainServices(this IServiceCollection services)
        {
            //services.AddScoped<IUnitOfWork, UnitOfWork>();
            //services.AddScoped<ISP_Call, SP_Call>();
            //services.AddScoped<IEmailService, EmailService>();

            return services;
        }
    }
}
