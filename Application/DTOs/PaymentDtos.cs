using System;
using System.ComponentModel.DataAnnotations;

namespace Application.DTOs
{
    public class PaymentRequestDto
    {
        [Required]
        public decimal Amount { get; set; }

        [Required]
        public string Currency { get; set; } = "usd";

        [Required]
        public string Description { get; set; }

        public string CustomerEmail { get; set; }
    }

    public class PaymentResponseDto
    {
        public bool IsSuccess { get; set; }
        public string Message { get; set; }
        public string PaymentIntentId { get; set; }
        public string ClientSecret { get; set; }
        public decimal Amount { get; set; }
        public string Currency { get; set; }
        public string Status { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class StripePaymentDto
    {
        [Required]
        public decimal Amount { get; set; }

        [Required]
        public string Currency { get; set; } = "usd";

        [Required]
        public string Description { get; set; }

        // Test card details (hardcoded for test mode)
        public string CardNumber { get; set; } = "4242424242424242";
        public string ExpiryMonth { get; set; } = "12";
        public string ExpiryYear { get; set; } = "2025";
        public string Cvc { get; set; } = "123";

        public string CustomerEmail { get; set; }
    }

    public class PaymentConfirmationDto
    {
        [Required]
        public string PaymentIntentId { get; set; }

        public bool IsSuccess { get; set; }
        public string Status { get; set; }
        public string Message { get; set; }
        public decimal Amount { get; set; }
        public string Currency { get; set; }
        public DateTime ConfirmedAt { get; set; }
    }

    public class PickupPaymentDto
    {
        [Required]
        public Guid PickupRequestId { get; set; }

        [Required]
        [Range(0.01, double.MaxValue)]
        public decimal Amount { get; set; }

        [Required]
        public string Currency { get; set; } = "usd";
    }

    public class PurchasePaymentDto
    {
        [Required]
        public Guid PurchaseId { get; set; }

        [Required]
        public int FactoryId { get; set; }

        [Required]
        [Range(0.01, double.MaxValue)]
        public decimal Amount { get; set; }

        [Required]
        public string Currency { get; set; } = "usd";
    }

    public class PaymentLogDto
    {
        public Guid Id { get; set; }
        public string PaymentIntentId { get; set; }
        public decimal Amount { get; set; }
        public string Currency { get; set; }
        public string Status { get; set; }
        public string Description { get; set; }
        public string CustomerEmail { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? ConfirmedAt { get; set; }
        public string ErrorMessage { get; set; }
    }
}
