using System.Threading.Tasks;
using Domain.Entities;

namespace Application.Contracts
{
    public interface IEmailService
    {
        // User Notifications
        Task<bool> SendPickupRequestConfirmationAsync(string userEmail, PickupRequest request);
        Task<bool> SendPickupApprovalAsync(string userEmail, PickupRequest request, decimal paymentAmount);
        Task<bool> SendPickupRejectionAsync(string userEmail, PickupRequest request, string reason);
        Task<bool> SendPaymentConfirmationAsync(string userEmail, decimal amount, string description);
        Task<bool> SendItemSoldNotificationAsync(string userEmail, MarketplaceItem item, Purchase purchase);

        // Admin Notifications
        Task<bool> SendAdminNewRequestAsync(string adminEmail, PickupRequest request);
        Task<bool> SendPaymentProcessingConfirmationAsync(string adminEmail, Payment payment);

        // Factory Notifications
        Task<bool> SendPurchaseConfirmationAsync(string factoryEmail, Purchase purchase);
        Task<bool> SendPaymentReceiptAsync(string factoryEmail, Payment payment);

        // Utility Methods
        Task<bool> SendCustomEmailAsync(string toEmail, string subject, string htmlContent);
        Task<bool> SendBulkEmailAsync(string[] toEmails, string subject, string htmlContent);
    }
}
