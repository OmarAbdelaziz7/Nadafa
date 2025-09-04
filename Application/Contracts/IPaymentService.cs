using System;
using System.Threading.Tasks;
using Application.DTOs;

namespace Application.Contracts
{
    public interface IPaymentService
    {
        /// <summary>
        /// Process payment for approved pickup request (Admin pays user)
        /// </summary>
        Task<PaymentResponseDto> ProcessPickupPaymentAsync(Guid pickupRequestId, decimal amount);

        /// <summary>
        /// Process payment for factory purchasing marketplace items
        /// </summary>
        Task<PaymentResponseDto> ProcessPurchasePaymentAsync(Guid purchaseId, int factoryId);

        /// <summary>
        /// Create a payment intent with Stripe
        /// </summary>
        Task<PaymentResponseDto> CreatePaymentIntentAsync(decimal amount, string currency = "usd", string description = null, string customerEmail = null);

        /// <summary>
        /// Confirm a payment intent
        /// </summary>
        Task<PaymentConfirmationDto> ConfirmPaymentAsync(string paymentIntentId);

        /// <summary>
        /// Process payment with test card details
        /// </summary>
        Task<PaymentResponseDto> ProcessTestPaymentAsync(StripePaymentDto paymentDto);

        /// <summary>
        /// Get payment status
        /// </summary>
        Task<PaymentConfirmationDto> GetPaymentStatusAsync(string paymentIntentId);
    }
}
