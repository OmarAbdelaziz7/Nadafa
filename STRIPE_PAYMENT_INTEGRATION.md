# NADAFA Stripe Payment Integration

## Overview

This document provides comprehensive information about the Stripe payment integration implemented in the NADAFA recycling platform. The integration handles two main payment scenarios:

1. **Admin paying users for approved pickups** - Automatic payment when admin approves pickup requests
2. **Factories purchasing marketplace items** - Direct payment from factory to seller

## Architecture

### Payment Flow

```
┌─────────────────┐    ┌─────────────────┐    ┌─────────────────┐
│   User Request  │───▶│  Admin Approval │───▶│  Auto Payment   │
│   Pickup        │    │                 │    │   to User       │
└─────────────────┘    └─────────────────┘    └─────────────────┘
                                │
                                ▼
                       ┌─────────────────┐
                       │ Marketplace     │
                       │ Item Created    │
                       └─────────────────┘
                                │
                                ▼
                       ┌─────────────────┐    ┌─────────────────┐
                       │ Factory Buys    │───▶│ Direct Payment  │
                       │ Item            │    │ to Seller       │
                       └─────────────────┘    └─────────────────┘
```

### Key Components

1. **IPaymentService** - Payment service interface
2. **StripePaymentService** - Stripe implementation
3. **PaymentController** - API endpoints
4. **Payment DTOs** - Data transfer objects
5. **Updated Services** - PickupRequestService and PurchaseService

## Configuration

### Environment Variables

```json
{
  "Stripe": {
    "SecretKey": "",
    "PublicKey": "",
    "WebhookSecret": "whsec_test_webhook_secret"
  }
}
```

### Test Card Details

For testing purposes, the following hardcoded test card details are used:

- **Card Number**: 4242424242424242
- **Expiry Month**: 12
- **Expiry Year**: 2025
- **CVC**: 123

## API Endpoints

### 1. Process Pickup Payment (Admin)

```http
POST /api/payment/pickup/{pickupRequestId}
Authorization: Bearer {admin_token}
Content-Type: application/json

{
  "amount": 150.00,
  "currency": "usd"
}
```

**Response:**
```json
{
  "isSuccess": true,
  "message": "Payment processed successfully",
  "paymentIntentId": "pi_3OqX8n2eZvKYlo2C1gQKqX8n",
  "clientSecret": "pi_3OqX8n2eZvKYlo2C1gQKqX8n_secret_...",
  "amount": 150.00,
  "currency": "usd",
  "status": "succeeded",
  "createdAt": "2024-01-15T10:30:00Z"
}
```

### 2. Process Purchase Payment (Factory)

```http
POST /api/payment/purchase/{purchaseId}
Authorization: Bearer {factory_token}
Content-Type: application/json

{
  "factoryId": 123,
  "amount": 200.00,
  "currency": "usd"
}
```

### 3. Create Payment Intent

```http
POST /api/payment/create-intent
Authorization: Bearer {token}
Content-Type: application/json

{
  "amount": 100.00,
  "currency": "usd",
  "description": "Test payment",
  "customerEmail": "customer@example.com"
}
```

### 4. Process Test Payment

```http
POST /api/payment/test
Content-Type: application/json

{
  "amount": 50.00,
  "currency": "usd",
  "description": "Test payment with hardcoded card",
  "customerEmail": "test@example.com"
}
```

### 5. Get Payment Status

```http
GET /api/payment/status/{paymentIntentId}
Authorization: Bearer {token}
```

### 6. Stripe Webhook

```http
POST /api/payment/webhook
Content-Type: application/json
Stripe-Signature: {webhook_signature}

{
  "id": "evt_3OqX8n2eZvKYlo2C1gQKqX8n",
  "type": "payment_intent.succeeded",
  "data": {
    "object": {
      "id": "pi_3OqX8n2eZvKYlo2C1gQKqX8n",
      "amount": 5000,
      "currency": "usd",
      "status": "succeeded"
    }
  }
}
```

## Testing Scenarios

### 1. Test Pickup Payment Flow

```bash
# 1. Create pickup request
curl -X POST "https://localhost:5294/api/pickup-requests" \
  -H "Authorization: Bearer {user_token}" \
  -H "Content-Type: application/json" \
  -d '{
    "materialType": 1,
    "quantity": 10.5,
    "unit": 1,
    "proposedPricePerUnit": 15.00,
    "description": "Paper recycling pickup"
  }'

# 2. Approve pickup request (triggers automatic payment)
curl -X POST "https://localhost:5294/api/pickup-requests/{requestId}/approve" \
  -H "Authorization: Bearer {admin_token}" \
  -H "Content-Type: application/json" \
  -d '{
    "notes": "Approved for pickup"
  }'
```

### 2. Test Factory Purchase Flow

```bash
# 1. Create purchase
curl -X POST "https://localhost:5294/api/purchases" \
  -H "Authorization: Bearer {factory_token}" \
  -H "Content-Type: application/json" \
  -d '{
    "marketplaceItemId": "{itemId}",
    "quantity": 5.0
  }'

# 2. Process payment
curl -X POST "https://localhost:5294/api/payment/purchase/{purchaseId}" \
  -H "Authorization: Bearer {factory_token}" \
  -H "Content-Type: application/json" \
  -d '{
    "factoryId": 123,
    "amount": 75.00,
    "currency": "usd"
  }'
```

### 3. Test Direct Payment

```bash
# Test payment with hardcoded card
curl -X POST "https://localhost:5294/api/payment/test" \
  -H "Content-Type: application/json" \
  -d '{
    "amount": 25.00,
    "currency": "usd",
    "description": "Test payment",
    "customerEmail": "test@example.com"
  }'
```

## Postman Collection

### Environment Variables

```json
{
  "baseUrl": "https://localhost:5294",
  "userToken": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "adminToken": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "factoryToken": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9..."
}
```

### Test Requests

1. **Create Pickup Request**
   - Method: POST
   - URL: {{baseUrl}}/api/pickup-requests
   - Headers: Authorization: Bearer {{userToken}}
   - Body: Raw JSON with pickup request data

2. **Approve Pickup Request**
   - Method: POST
   - URL: {{baseUrl}}/api/pickup-requests/{{requestId}}/approve
   - Headers: Authorization: Bearer {{adminToken}}
   - Body: Raw JSON with approval notes

3. **Create Purchase**
   - Method: POST
   - URL: {{baseUrl}}/api/purchases
   - Headers: Authorization: Bearer {{factoryToken}}
   - Body: Raw JSON with purchase data

4. **Process Test Payment**
   - Method: POST
   - URL: {{baseUrl}}/api/payment/test
   - Body: Raw JSON with test payment data

## Error Handling

### Common Error Responses

```json
{
  "error": "Pickup request not found"
}
```

```json
{
  "error": "Payment failed: Insufficient funds"
}
```

```json
{
  "error": "An error occurred while processing the payment"
}
```

### Error Codes

- **400 Bad Request**: Invalid input data
- **401 Unauthorized**: Missing or invalid token
- **403 Forbidden**: Insufficient permissions
- **404 Not Found**: Resource not found
- **500 Internal Server Error**: Server error

## Webhook Testing

### Using Stripe CLI

```bash
# Install Stripe CLI
# Download from: https://stripe.com/docs/stripe-cli

# Login to Stripe
stripe login

# Forward webhooks to local server
stripe listen --forward-to localhost:5294/api/payment/webhook

# Trigger test webhook
stripe trigger payment_intent.succeeded
```

### Webhook Events Handled

1. **payment_intent.succeeded** - Payment completed successfully
2. **payment_intent.payment_failed** - Payment failed
3. **payment_intent.canceled** - Payment canceled

## Security Considerations

### 1. API Key Security
- Store Stripe secret key in environment variables
- Never expose secret key in client-side code
- Use different keys for test and production

### 2. Webhook Security
- Verify webhook signatures
- Use HTTPS in production
- Implement idempotency for webhook processing

### 3. Payment Validation
- Validate payment amounts server-side
- Check user permissions before processing payments
- Implement proper error handling and rollback

## Monitoring and Logging

### Payment Logs

All payment operations are logged with the following information:
- Payment Intent ID
- Amount and currency
- Status
- Description
- Customer email
- Timestamp

### Log Levels

- **Information**: Successful payments, webhook events
- **Warning**: Payment failures, validation errors
- **Error**: System errors, Stripe API errors

## Production Deployment

### 1. Environment Setup

```bash
# Set production Stripe keys
export STRIPE_SECRET_KEY="sk_live_..."
export STRIPE_PUBLIC_KEY="pk_live_..."
export STRIPE_WEBHOOK_SECRET="whsec_..."
```

### 2. Webhook Configuration

1. Create webhook endpoint in Stripe Dashboard
2. Set webhook URL to: `https://yourdomain.com/api/payment/webhook`
3. Select events: `payment_intent.succeeded`, `payment_intent.payment_failed`
4. Copy webhook secret to environment variables

### 3. SSL Certificate

Ensure HTTPS is enabled for webhook endpoints and payment processing.

## Troubleshooting

### Common Issues

1. **Payment Intent Creation Fails**
   - Check Stripe API key configuration
   - Verify amount format (cents vs dollars)
   - Check network connectivity

2. **Webhook Not Received**
   - Verify webhook URL is accessible
   - Check webhook secret configuration
   - Ensure HTTPS is enabled

3. **Payment Confirmation Fails**
   - Verify payment intent ID
   - Check payment status in Stripe Dashboard
   - Review error logs

### Debug Mode

Enable debug logging by setting log level to Debug:

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Debug",
      "Infrastructure.Services.StripePaymentService": "Debug"
    }
  }
}
```

## Support

For Stripe-related issues:
- Stripe Documentation: https://stripe.com/docs
- Stripe Support: https://support.stripe.com
- Stripe Status: https://status.stripe.com

For NADAFA platform issues:
- Check application logs
- Review error responses
- Contact development team
