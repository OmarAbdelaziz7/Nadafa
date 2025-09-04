using System;
using System.Threading.Tasks;
using Application.Contracts;
using Application.DTOs;
using Infrastructure.Repositories;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Stripe;

namespace Application.Implementations
{
    public class StripePaymentService : IPaymentService
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<StripePaymentService> _logger;
        private readonly IPickupRequestRepository _pickupRequestRepository;
        private readonly IPurchaseRepository _purchaseRepository;
        private readonly IMarketplaceRepository _marketplaceRepository;
        private readonly INotificationRepository _notificationRepository;
        private readonly IEmailService _emailService;

        public StripePaymentService(
            IConfiguration configuration,
            ILogger<StripePaymentService> logger,
            IPickupRequestRepository pickupRequestRepository,
            IPurchaseRepository purchaseRepository,
            IMarketplaceRepository marketplaceRepository,
            INotificationRepository notificationRepository,
            IEmailService emailService)
        {
            _configuration = configuration;
            _logger = logger;
            _pickupRequestRepository = pickupRequestRepository;
            _purchaseRepository = purchaseRepository;
            _marketplaceRepository = marketplaceRepository;
            _notificationRepository = notificationRepository;
            _emailService = emailService;

            // Configure Stripe with test secret key
            StripeConfiguration.ApiKey = _configuration["Stripe:SecretKey"];
        }

        public async Task<PaymentResponseDto> ProcessPickupPaymentAsync(Guid pickupRequestId, decimal amount)
        {
            try
            {
                _logger.LogInformation($"Processing pickup payment for request {pickupRequestId} with amount {amount}");

                // Get pickup request
                var pickupRequest = await _pickupRequestRepository.GetByIdAsync(pickupRequestId);
                if (pickupRequest == null)
                {
                    throw new ArgumentException($"Pickup request {pickupRequestId} not found");
                }

                if (pickupRequest.Status != Domain.Entities.PickupStatus.Approved)
                {
                    throw new InvalidOperationException($"Pickup request {pickupRequestId} is not approved");
                }

                // Create payment intent
                var paymentIntent = await CreateStripePaymentIntentAsync(amount, "usd",
                    $"Payment for pickup request {pickupRequestId}");

                // Log payment
                await LogPaymentAsync(paymentIntent.Id, amount, "usd", "succeeded",
                    $"Pickup payment for request {pickupRequestId}", pickupRequest.User?.Email);

                // Send email confirmation
                try
                {
                    await _emailService.SendPaymentConfirmationAsync(
                        pickupRequest.User?.Email ?? "user@example.com",
                        amount,
                        $"Pickup payment for request {pickupRequestId}");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to send payment confirmation email");
                }

                _logger.LogInformation($"Successfully processed pickup payment. PaymentIntentId: {paymentIntent.Id}");

                return new PaymentResponseDto
                {
                    IsSuccess = true,
                    Message = "Payment processed successfully",
                    PaymentIntentId = paymentIntent.Id,
                    ClientSecret = paymentIntent.ClientSecret,
                    Amount = amount,
                    Currency = "usd",
                    Status = paymentIntent.Status,
                    CreatedAt = DateTime.UtcNow
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error processing pickup payment for request {pickupRequestId}");
                throw;
            }
        }

        public async Task<PaymentResponseDto> ProcessPurchasePaymentAsync(Guid purchaseId, int factoryId)
        {
            try
            {
                _logger.LogInformation($"Processing purchase payment for purchase {purchaseId} by factory {factoryId}");

                // Get purchase
                var purchase = await _purchaseRepository.GetByIdAsync(purchaseId);
                if (purchase == null)
                {
                    throw new ArgumentException($"Purchase {purchaseId} not found");
                }

                if (purchase.FactoryId != factoryId)
                {
                    throw new InvalidOperationException($"Purchase {purchaseId} does not belong to factory {factoryId}");
                }

                // Create payment intent
                var paymentIntent = await CreateStripePaymentIntentAsync(purchase.TotalAmount, "usd",
                    $"Payment for purchase {purchaseId}");

                // Update purchase with payment intent ID
                purchase.StripePaymentIntentId = paymentIntent.Id;
                purchase.PaymentStatus = Domain.Entities.PaymentStatus.Completed;
                purchase.UpdateTimestamp();
                await _purchaseRepository.UpdateAsync(purchase);

                // Update marketplace item availability
                var marketplaceItem = await _marketplaceRepository.GetByIdAsync(purchase.MarketplaceItemId);
                if (marketplaceItem != null)
                {
                    marketplaceItem.Quantity -= purchase.Quantity;
                    if (marketplaceItem.Quantity <= 0)
                    {
                        marketplaceItem.IsAvailable = false;
                    }
                    marketplaceItem.UpdateTimestamp();
                    await _marketplaceRepository.UpdateAsync(marketplaceItem);
                }

                // Log payment
                await LogPaymentAsync(paymentIntent.Id, purchase.TotalAmount, "usd", "succeeded",
                    $"Purchase payment for item {purchase.MarketplaceItemId}", purchase.Factory?.Email);

                // Send email notifications
                try
                {
                    // Send purchase confirmation to factory
                    await _emailService.SendPurchaseConfirmationAsync(
                        purchase.Factory?.Email ?? "factory@example.com",
                        purchase);

                    // Send payment receipt to factory
                    await _emailService.SendPaymentReceiptAsync(
                        purchase.Factory?.Email ?? "factory@example.com",
                        new Domain.Entities.Payment
                        {
                            Id = Guid.NewGuid(),
                            Amount = purchase.TotalAmount,
                            PaymentMethod = "Stripe",
                            Status = Domain.Entities.PaymentStatus.Completed,
                            PaymentDate = DateTime.UtcNow
                        });
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to send purchase confirmation emails");
                }

                _logger.LogInformation($"Successfully processed purchase payment. PaymentIntentId: {paymentIntent.Id}");

                return new PaymentResponseDto
                {
                    IsSuccess = true,
                    Message = "Purchase payment processed successfully",
                    PaymentIntentId = paymentIntent.Id,
                    ClientSecret = paymentIntent.ClientSecret,
                    Amount = purchase.TotalAmount,
                    Currency = "usd",
                    Status = paymentIntent.Status,
                    CreatedAt = DateTime.UtcNow
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error processing purchase payment for purchase {purchaseId}");
                throw;
            }
        }

        public async Task<PaymentResponseDto> CreatePaymentIntentAsync(decimal amount, string currency = "usd", string description = null, string customerEmail = null)
        {
            try
            {
                var paymentIntent = await CreateStripePaymentIntentAsync(amount, currency, description);

                return new PaymentResponseDto
                {
                    IsSuccess = true,
                    Message = "Payment intent created successfully",
                    PaymentIntentId = paymentIntent.Id,
                    ClientSecret = paymentIntent.ClientSecret,
                    Amount = amount,
                    Currency = currency,
                    Status = paymentIntent.Status,
                    CreatedAt = DateTime.UtcNow
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating payment intent");
                throw;
            }
        }

        public async Task<PaymentConfirmationDto> ConfirmPaymentAsync(string paymentIntentId)
        {
            try
            {
                var paymentIntentService = new PaymentIntentService();
                var paymentIntent = await paymentIntentService.GetAsync(paymentIntentId);

                var confirmation = new PaymentConfirmationDto
                {
                    PaymentIntentId = paymentIntentId,
                    IsSuccess = paymentIntent.Status == "succeeded",
                    Status = paymentIntent.Status,
                    Message = paymentIntent.Status == "succeeded" ? "Payment confirmed successfully" : "Payment not confirmed",
                    Amount = paymentIntent.Amount / 100m, // Convert from cents
                    Currency = paymentIntent.Currency,
                    ConfirmedAt = DateTime.UtcNow
                };

                // Log payment confirmation
                await LogPaymentAsync(paymentIntentId, confirmation.Amount, confirmation.Currency,
                    confirmation.Status, "Payment confirmation", null);

                return confirmation;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error confirming payment {paymentIntentId}");
                throw;
            }
        }

        public async Task<PaymentResponseDto> ProcessTestPaymentAsync(StripePaymentDto paymentDto)
        {
            try
            {
                _logger.LogInformation($"Processing test payment for amount {paymentDto.Amount}");

                // Create payment intent
                var paymentIntent = await CreateStripePaymentIntentAsync(paymentDto.Amount, paymentDto.Currency, paymentDto.Description);

                // Create payment method with test card
                var paymentMethodService = new PaymentMethodService();
                var paymentMethod = await paymentMethodService.CreateAsync(new PaymentMethodCreateOptions
                {
                    Type = "card",
                    Card = new PaymentMethodCardOptions
                    {
                        Number = paymentDto.CardNumber,
                        ExpMonth = int.Parse(paymentDto.ExpiryMonth),
                        ExpYear = int.Parse(paymentDto.ExpiryYear),
                        Cvc = paymentDto.Cvc
                    }
                });

                // Attach payment method to payment intent
                var paymentIntentService = new PaymentIntentService();
                await paymentIntentService.ConfirmAsync(paymentIntent.Id, new PaymentIntentConfirmOptions
                {
                    PaymentMethod = paymentMethod.Id
                });

                // Get updated payment intent
                var updatedPaymentIntent = await paymentIntentService.GetAsync(paymentIntent.Id);

                // Log payment
                await LogPaymentAsync(paymentIntent.Id, paymentDto.Amount, paymentDto.Currency,
                    updatedPaymentIntent.Status, paymentDto.Description, paymentDto.CustomerEmail);

                return new PaymentResponseDto
                {
                    IsSuccess = updatedPaymentIntent.Status == "succeeded",
                    Message = updatedPaymentIntent.Status == "succeeded" ? "Test payment processed successfully" : "Test payment failed",
                    PaymentIntentId = paymentIntent.Id,
                    ClientSecret = paymentIntent.ClientSecret,
                    Amount = paymentDto.Amount,
                    Currency = paymentDto.Currency,
                    Status = updatedPaymentIntent.Status,
                    CreatedAt = DateTime.UtcNow
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing test payment");
                throw;
            }
        }

        public async Task<PaymentConfirmationDto> GetPaymentStatusAsync(string paymentIntentId)
        {
            try
            {
                var paymentIntentService = new PaymentIntentService();
                var paymentIntent = await paymentIntentService.GetAsync(paymentIntentId);

                return new PaymentConfirmationDto
                {
                    PaymentIntentId = paymentIntentId,
                    IsSuccess = paymentIntent.Status == "succeeded",
                    Status = paymentIntent.Status,
                    Message = paymentIntent.Status == "succeeded" ? "Payment successful" : "Payment not successful",
                    Amount = paymentIntent.Amount / 100m, // Convert from cents
                    Currency = paymentIntent.Currency,
                    ConfirmedAt = DateTime.UtcNow
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting payment status for {paymentIntentId}");
                throw;
            }
        }

        private async Task<PaymentIntent> CreateStripePaymentIntentAsync(decimal amount, string currency, string description)
        {
            var paymentIntentService = new PaymentIntentService();
            var options = new PaymentIntentCreateOptions
            {
                Amount = (long)(amount * 100), // Convert to cents
                Currency = currency,
                Description = description,
                AutomaticPaymentMethods = new PaymentIntentAutomaticPaymentMethodsOptions
                {
                    Enabled = true
                }
            };

            return await paymentIntentService.CreateAsync(options);
        }

        private async Task LogPaymentAsync(string paymentIntentId, decimal amount, string currency, string status, string description, string customerEmail)
        {
            try
            {
                // This would typically log to a payment log table
                _logger.LogInformation($"Payment logged - ID: {paymentIntentId}, Amount: {amount} {currency}, Status: {status}, Description: {description}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error logging payment");
            }
        }
    }
}
