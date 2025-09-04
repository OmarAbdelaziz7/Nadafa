using System;
using System.Threading.Tasks;
using Application.Contracts;
using Application.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Stripe;
using Presentation.Base;

namespace Presentation.Controllers
{
    public class PaymentController : ApiControllerBase
    {
        private readonly IPaymentService _paymentService;
        private readonly ILogger<PaymentController> _logger;

        public PaymentController(IPaymentService paymentService, ILogger<PaymentController> logger)
        {
            _paymentService = paymentService;
            _logger = logger;
        }

        /// <summary>
        /// Process payment for approved pickup request (Admin pays user)
        /// </summary>
        [HttpPost("pickup/{pickupRequestId}")]
        [AuthorizeRoles("Admin")]
        public async Task<ActionResult<PaymentResponseDto>> ProcessPickupPayment(
            Guid pickupRequestId,
            [FromBody] PickupPaymentDto paymentDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                _logger.LogInformation($"Processing pickup payment for request {pickupRequestId}");

                var result = await _paymentService.ProcessPickupPaymentAsync(pickupRequestId, paymentDto.Amount);

                return Ok(result);
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, $"Invalid request for pickup payment {pickupRequestId}");
                return BadRequest(new { error = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex, $"Invalid operation for pickup payment {pickupRequestId}");
                return BadRequest(new { error = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error processing pickup payment for request {pickupRequestId}");
                return HandleException(ex, "An error occurred while processing the payment");
            }
        }

        /// <summary>
        /// Process payment for factory purchasing marketplace items
        /// </summary>
        [HttpPost("purchase/{purchaseId}")]
        [AuthorizeRoles("Factory")]
        public async Task<ActionResult<PaymentResponseDto>> ProcessPurchasePayment(
            Guid purchaseId,
            [FromBody] PurchasePaymentDto paymentDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                _logger.LogInformation($"Processing purchase payment for purchase {purchaseId}");

                var result = await _paymentService.ProcessPurchasePaymentAsync(purchaseId, paymentDto.FactoryId);

                return Ok(result);
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, $"Invalid request for purchase payment {purchaseId}");
                return BadRequest(new { error = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex, $"Invalid operation for purchase payment {purchaseId}");
                return BadRequest(new { error = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error processing purchase payment for purchase {purchaseId}");
                return HandleException(ex, "An error occurred while processing the payment");
            }
        }

        /// <summary>
        /// Create a payment intent
        /// </summary>
        [HttpPost("create-intent")]
        public async Task<ActionResult<PaymentResponseDto>> CreatePaymentIntent([FromBody] PaymentRequestDto request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                _logger.LogInformation($"Creating payment intent for amount {request.Amount} {request.Currency}");

                var result = await _paymentService.CreatePaymentIntentAsync(
                    request.Amount,
                    request.Currency,
                    request.Description,
                    request.CustomerEmail);

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating payment intent");
                return HandleException(ex, "An error occurred while creating the payment intent");
            }
        }

        /// <summary>
        /// Confirm a payment
        /// </summary>
        [HttpPost("confirm")]
        public async Task<ActionResult<PaymentConfirmationDto>> ConfirmPayment([FromBody] PaymentConfirmationDto request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                _logger.LogInformation($"Confirming payment {request.PaymentIntentId}");

                var result = await _paymentService.ConfirmPaymentAsync(request.PaymentIntentId);

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error confirming payment {request.PaymentIntentId}");
                return HandleException(ex, "An error occurred while confirming the payment");
            }
        }

        /// <summary>
        /// Process test payment with hardcoded card details
        /// </summary>
        [HttpPost("test")]
        [AllowAnonymous]
        public async Task<ActionResult<PaymentResponseDto>> ProcessTestPayment([FromBody] StripePaymentDto paymentDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                _logger.LogInformation($"Processing test payment for amount {paymentDto.Amount}");

                var result = await _paymentService.ProcessTestPaymentAsync(paymentDto);

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing test payment");
                return HandleException(ex, "An error occurred while processing the test payment");
            }
        }

        /// <summary>
        /// Get payment status
        /// </summary>
        [HttpGet("status/{paymentIntentId}")]
        public async Task<ActionResult<PaymentConfirmationDto>> GetPaymentStatus(string paymentIntentId)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(paymentIntentId))
                {
                    return BadRequest(new { error = "Payment intent ID is required" });
                }

                _logger.LogInformation($"Getting payment status for {paymentIntentId}");

                var result = await _paymentService.GetPaymentStatusAsync(paymentIntentId);

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting payment status for {paymentIntentId}");
                return HandleException(ex, "An error occurred while getting the payment status");
            }
        }

        /// <summary>
        /// Stripe webhook endpoint for payment events
        /// </summary>
        [HttpPost("webhook")]
        [AllowAnonymous]
        public async Task<IActionResult> StripeWebhook()
        {
            try
            {
                var json = await new System.IO.StreamReader(Request.Body).ReadToEndAsync();
                var stripeEvent = EventUtility.ConstructEvent(json, Request.Headers["Stripe-Signature"],
                    GetStripeWebhookSecret());

                _logger.LogInformation($"Received Stripe webhook event: {stripeEvent.Type}");

                switch (stripeEvent.Type)
                {
                    case Events.PaymentIntentSucceeded:
                        var paymentIntent = stripeEvent.Data.Object as Stripe.PaymentIntent;
                        _logger.LogInformation($"Payment succeeded: {paymentIntent.Id}");
                        break;

                    case Events.PaymentIntentPaymentFailed:
                        var failedPayment = stripeEvent.Data.Object as Stripe.PaymentIntent;
                        _logger.LogWarning($"Payment failed: {failedPayment.Id}");
                        break;

                    default:
                        _logger.LogInformation($"Unhandled event type: {stripeEvent.Type}");
                        break;
                }

                return Ok();
            }
            catch (StripeException ex)
            {
                _logger.LogError(ex, "Stripe webhook error");
                return BadRequest();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing Stripe webhook");
                return StatusCode(500);
            }
        }

        private string GetStripeWebhookSecret()
        {
            // In production, this should come from configuration
            return "whsec_test_webhook_secret";
        }
    }
}
