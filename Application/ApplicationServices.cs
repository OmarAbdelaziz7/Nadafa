using Application.Contracts;
using Application.Implementations;
using Microsoft.Extensions.DependencyInjection;

namespace Application
{
    public static class ApplicationServices
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            // Auth Services
            services.AddScoped<IAuthService, AuthService>();

            // Pickup Request Services
            services.AddScoped<IPickupRequestService, PickupRequestService>();

            // Marketplace Services
            services.AddScoped<IMarketplaceService, MarketplaceService>();

            // Purchase Services
            services.AddScoped<IPurchaseService, PurchaseService>();

            // Notification Services
            services.AddScoped<INotificationService, NotificationService>();

            return services;
        }
    }
}
