# Email Testing Guide - NADAFA Platform

## Quick Start

This guide will help you test the SendGrid email notification system in the NADAFA platform.

## Prerequisites

1. **SendGrid Account**: Ensure you have a SendGrid account with API access
2. **API Key**: Your SendGrid API key is configured in `appsettings.json`
3. **Verified Sender**: The sender email is verified in SendGrid

## Configuration Check

Verify your `appsettings.json` contains:

```json
{
  "SendGridSettings": {
    "ApiKey": "",
    "FromEmail": "islamhk1234@gmail.com",
    "FromName": "NADAFA Recycling Platform"
  }
}
```

## Testing Scenarios

### 1. Pickup Request Confirmation

**Trigger**: Create a new pickup request
**Expected Email**: Confirmation email with request details

```csharp
// Test in PickupRequestService.CreateRequestAsync
var result = await _emailService.SendPickupRequestConfirmationAsync(
    "test@example.com", 
    pickupRequest);
```

### 2. Pickup Approval

**Trigger**: Approve a pickup request
**Expected Email**: Approval email with payment information

```csharp
// Test in PickupRequestService.ApproveRequestAsync
var result = await _emailService.SendPickupApprovalAsync(
    "user@example.com", 
    pickupRequest, 
    1250.00m);
```

### 3. Payment Confirmation

**Trigger**: Process a payment
**Expected Email**: Payment confirmation with transaction details

```csharp
// Test in StripePaymentService.ProcessPickupPaymentAsync
var result = await _emailService.SendPaymentConfirmationAsync(
    "user@example.com", 
    1250.00m, 
    "Pickup payment for request PR-2024-001234");
```

### 4. Item Sold Notification

**Trigger**: Complete a marketplace purchase
**Expected Email**: Item sold notification to original seller

```csharp
// Test in PurchaseService
var result = await _emailService.SendItemSoldNotificationAsync(
    "seller@example.com", 
    marketplaceItem, 
    purchase);
```

## Manual Testing

### 1. Create Test Controller

Add this test controller to test email functionality:

```csharp
[ApiController]
[Route("api/[controller]")]
public class EmailTestController : ControllerBase
{
    private readonly IEmailService _emailService;

    public EmailTestController(IEmailService emailService)
    {
        _emailService = emailService;
    }

    [HttpPost("test-pickup-confirmation")]
    public async Task<IActionResult> TestPickupConfirmation([FromBody] string email)
    {
        var pickupRequest = new PickupRequest
        {
            Id = Guid.NewGuid(),
            UserId = 1,
            MaterialType = MaterialType.Plastic,
            Quantity = 500,
            Unit = Unit.Kg,
            ProposedPricePerUnit = 2.50m,
            Description = "Clean plastic bottles and containers",
            Status = PickupStatus.Pending,
            RequestDate = DateTime.UtcNow
        };

        var result = await _emailService.SendPickupRequestConfirmationAsync(email, pickupRequest);
        return Ok(new { Success = result, Message = "Test email sent" });
    }

    [HttpPost("test-payment-confirmation")]
    public async Task<IActionResult> TestPaymentConfirmation([FromBody] string email)
    {
        var result = await _emailService.SendPaymentConfirmationAsync(email, 1250.00m, "Test payment");
        return Ok(new { Success = result, Message = "Test email sent" });
    }
}
```

### 2. Test with HTTP Requests

```http
### Test Pickup Confirmation
POST http://localhost:5000/api/EmailTest/test-pickup-confirmation
Content-Type: application/json

"your-email@example.com"

### Test Payment Confirmation
POST http://localhost:5000/api/EmailTest/test-payment-confirmation
Content-Type: application/json

"your-email@example.com"
```

## Monitoring Email Delivery

### 1. SendGrid Dashboard

1. Log into your SendGrid account
2. Go to **Activity** → **Email Activity**
3. Monitor email delivery status
4. Check for bounces or spam reports

### 2. Application Logs

Check application logs for email delivery status:

```
info: Infrastructure.Services.SendGridEmailService[0]
      Email sent to user@example.com. Status: Accepted
```

### 3. Common Status Codes

- **202 Accepted**: Email accepted for delivery
- **400 Bad Request**: Invalid request (check API key)
- **401 Unauthorized**: Invalid API key
- **403 Forbidden**: Insufficient permissions

## Troubleshooting

### Email Not Received

1. **Check Spam Folder**: Emails might be in spam/junk folder
2. **Verify Sender**: Ensure sender email is verified in SendGrid
3. **Check API Key**: Verify API key is correct and has proper permissions
4. **Monitor Logs**: Check application logs for errors

### Common Errors

1. **401 Unauthorized**
   - Invalid or expired API key
   - Solution: Regenerate API key in SendGrid

2. **400 Bad Request**
   - Invalid email address format
   - Missing required fields
   - Solution: Validate email addresses and request data

3. **403 Forbidden**
   - API key doesn't have send permissions
   - Solution: Update API key permissions in SendGrid

### Debug Steps

1. **Enable Detailed Logging**
   ```csharp
   _logger.LogInformation($"Sending email to {toEmail}");
   _logger.LogInformation($"Email response: {response.StatusCode}");
   ```

2. **Test API Key**
   ```csharp
   var client = new SendGridClient(apiKey);
   var response = await client.SendEmailAsync(message);
   Console.WriteLine($"Status: {response.StatusCode}");
   ```

3. **Validate Configuration**
   ```csharp
   var apiKey = _configuration["SendGridSettings:ApiKey"];
   var fromEmail = _configuration["SendGridSettings:FromEmail"];
   Console.WriteLine($"API Key: {!string.IsNullOrEmpty(apiKey)}");
   Console.WriteLine($"From Email: {fromEmail}");
   ```

## Best Practices

### 1. Test Environment

- Use test API keys for development
- Test with real email addresses
- Monitor delivery in SendGrid dashboard

### 2. Error Handling

- Don't fail operations if email fails
- Log errors for debugging
- Implement retry mechanisms

### 3. Email Content

- Test email templates in different email clients
- Verify mobile responsiveness
- Check spam score

## Production Checklist

- [ ] SendGrid API key is production-ready
- [ ] Sender email is verified and authenticated
- [ ] Email templates are tested and approved
- [ ] Error handling is implemented
- [ ] Monitoring is set up
- [ ] Rate limits are considered
- [ ] Backup email service is configured (optional)

## Support

For additional help:

1. **SendGrid Documentation**: https://docs.sendgrid.com/
2. **SendGrid Support**: Available in your SendGrid dashboard
3. **Application Logs**: Check detailed error messages
4. **Development Team**: Contact for platform-specific issues

## Sample Test Data

### Pickup Request
```json
{
  "id": "550e8400-e29b-41d4-a716-446655440000",
  "userId": 1,
  "materialType": "Plastic",
  "quantity": 500,
  "unit": "Kg",
  "proposedPricePerUnit": 2.50,
  "description": "Clean plastic bottles and containers",
  "status": "Pending",
  "requestDate": "2024-01-15T10:30:00Z"
}
```

### Payment Data
```json
{
  "amount": 1250.00,
  "description": "Pickup payment for request PR-2024-001234",
  "currency": "USD"
}
```

### Purchase Data
```json
{
  "id": "550e8400-e29b-41d4-a716-446655440001",
  "marketplaceItemId": "550e8400-e29b-41d4-a716-446655440002",
  "factoryId": 1,
  "quantity": 100,
  "pricePerUnit": 2.50,
  "totalAmount": 250.00,
  "purchaseDate": "2024-01-15T14:30:00Z"
}
```
