# NADAFA API Testing Guide

## Overview

This guide provides comprehensive testing instructions for the NADAFA recycling platform API. It includes step-by-step workflows, test scenarios, and troubleshooting tips to ensure proper API functionality.

## Prerequisites

### Required Tools
- **Postman** (recommended) or any REST API client
- **cURL** for command-line testing
- **JWT Token** for authentication
- **Test Data** for various scenarios

### Environment Setup
1. **Base URL**: `https://api.nadafa.com/v1`
2. **Content-Type**: `application/json`
3. **Authentication**: JWT Bearer token

## Authentication Testing

### 1. User Registration Test

**Endpoint**: `POST /api/auth/register/user`

**Test Data**:
```json
{
  "name": "Test User",
  "email": "test@example.com",
  "password": "TestPassword123!",
  "phone": "+1234567890",
  "address": "123 Test St, Test City"
}
```

**Expected Response**:
```json
{
  "isSuccess": true,
  "message": "User registered successfully",
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "user": {
    "id": 1,
    "name": "Test User",
    "email": "test@example.com",
    "role": "User"
  }
}
```

**Validation Points**:
- ✅ Status code: 200
- ✅ Token is present and valid format
- ✅ User role is "User"
- ✅ Email matches input

### 2. Factory Registration Test

**Endpoint**: `POST /api/auth/register/factory`

**Test Data**:
```json
{
  "name": "Green Recycling Factory",
  "email": "factory@example.com",
  "password": "TestPassword123!",
  "phone": "+1234567890",
  "address": "456 Industrial Ave, Test City",
  "businessLicense": "LIC123456",
  "capacity": 1000
}
```

**Expected Response**:
```json
{
  "isSuccess": true,
  "message": "Factory registered successfully",
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "user": {
    "id": 2,
    "name": "Green Recycling Factory",
    "email": "factory@example.com",
    "role": "Factory"
  }
}
```

**Validation Points**:
- ✅ Status code: 200
- ✅ Token is present and valid format
- ✅ User role is "Factory"
- ✅ Business license is included

### 3. Login Test

**Endpoint**: `POST /api/auth/login`

**Test Data**:
```json
{
  "email": "test@example.com",
  "password": "TestPassword123!"
}
```

**Expected Response**:
```json
{
  "isSuccess": true,
  "message": "Login successful",
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "user": {
    "id": 1,
    "name": "Test User",
    "email": "test@example.com",
    "role": "User"
  }
}
```

**Validation Points**:
- ✅ Status code: 200
- ✅ Token is present
- ✅ User information is correct
- ✅ Role matches registration

## Complete Workflow Testing

### Scenario 1: User Pickup Request Workflow

#### Step 1: Create Pickup Request

**Endpoint**: `POST /api/pickup-request`

**Headers**:
```
Authorization: Bearer <user-token>
Content-Type: application/json
```

**Request Body**:
```json
{
  "materialType": "Plastic",
  "quantity": 25.0,
  "unit": "Kg",
  "proposedPricePerUnit": 2.00,
  "description": "Clean plastic bottles and containers",
  "imageUrls": [
    "https://example.com/image1.jpg",
    "https://example.com/image2.jpg"
  ]
}
```

**Expected Response**:
```json
{
  "id": "550e8400-e29b-41d4-a716-446655440000",
  "userId": 1,
  "userName": "Test User",
  "materialType": "Plastic",
  "quantity": 25.0,
  "unit": "Kg",
  "proposedPricePerUnit": 2.00,
  "totalEstimatedPrice": 50.00,
  "description": "Clean plastic bottles and containers",
  "imageUrls": [
    "https://example.com/image1.jpg",
    "https://example.com/image2.jpg"
  ],
  "status": "Pending",
  "requestDate": "2024-01-15T10:30:00Z",
  "createdAt": "2024-01-15T10:30:00Z"
}
```

**Validation Points**:
- ✅ Status code: 201 (Created)
- ✅ Request ID is generated
- ✅ Status is "Pending"
- ✅ Total price is calculated correctly
- ✅ User ID matches authenticated user

#### Step 2: Admin Approve Request

**Endpoint**: `PUT /api/pickup-request/{request-id}/approve`

**Headers**:
```
Authorization: Bearer <admin-token>
Content-Type: application/json
```

**Request Body**:
```json
{
  "approvedPricePerUnit": 2.00,
  "notes": "Request approved after inspection"
}
```

**Expected Response**:
```json
{
  "id": "550e8400-e29b-41d4-a716-446655440000",
  "userId": 1,
  "userName": "Test User",
  "materialType": "Plastic",
  "quantity": 25.0,
  "unit": "Kg",
  "proposedPricePerUnit": 2.00,
  "approvedPricePerUnit": 2.00,
  "totalEstimatedPrice": 50.00,
  "description": "Clean plastic bottles and containers",
  "status": "Approved",
  "requestDate": "2024-01-15T10:30:00Z",
  "approvedAt": "2024-01-15T14:30:00Z",
  "approvedBy": "Admin User"
}
```

**Validation Points**:
- ✅ Status code: 200
- ✅ Status changed to "Approved"
- ✅ Approved timestamp is present
- ✅ Approved by field is populated

#### Step 3: Process Payment

**Endpoint**: `POST /api/payment/pickup/{request-id}`

**Headers**:
```
Authorization: Bearer <admin-token>
Content-Type: application/json
```

**Request Body**:
```json
{
  "amount": 50.00
}
```

**Expected Response**:
```json
{
  "isSuccess": true,
  "message": "Payment processed successfully",
  "paymentIntentId": "pi_1234567890",
  "clientSecret": "pi_1234567890_secret_abc123",
  "amount": 50.00,
  "currency": "usd",
  "status": "succeeded",
  "createdAt": "2024-01-15T14:30:00Z"
}
```

**Validation Points**:
- ✅ Status code: 200
- ✅ Payment intent ID is generated
- ✅ Amount matches request
- ✅ Status is "succeeded"

### Scenario 2: Marketplace Purchase Workflow

#### Step 1: Browse Marketplace Items

**Endpoint**: `GET /api/marketplace?materialType=Plastic&page=1&pageSize=10`

**Headers**:
```
Authorization: Bearer <user-token>
```

**Expected Response**:
```json
{
  "items": [
    {
      "id": "660e8400-e29b-41d4-a716-446655440000",
      "pickupRequestId": "550e8400-e29b-41d4-a716-446655440000",
      "userId": 1,
      "userName": "Test User",
      "materialType": "Plastic",
      "quantity": 25.0,
      "unit": "Kg",
      "pricePerUnit": 2.00,
      "totalPrice": 50.00,
      "description": "Clean plastic bottles and containers",
      "isAvailable": true,
      "publishedAt": "2024-01-15T15:00:00Z"
    }
  ],
  "totalCount": 1,
  "page": 1,
  "pageSize": 10,
  "totalPages": 1,
  "hasNextPage": false,
  "hasPreviousPage": false
}
```

**Validation Points**:
- ✅ Status code: 200
- ✅ Items array contains marketplace items
- ✅ Pagination metadata is correct
- ✅ Item is available for purchase

#### Step 2: Factory Purchase Item

**Endpoint**: `POST /api/marketplace/{item-id}/purchase`

**Headers**:
```
Authorization: Bearer <factory-token>
Content-Type: application/json
```

**Request Body**:
```json
{
  "marketplaceItemId": "660e8400-e29b-41d4-a716-446655440000",
  "quantity": 20.0,
  "pricePerUnit": 2.00,
  "stripePaymentIntentId": "pi_test123"
}
```

**Expected Response**:
```json
{
  "id": "770e8400-e29b-41d4-a716-446655440000",
  "marketplaceItemId": "660e8400-e29b-41d4-a716-446655440000",
  "factoryId": 2,
  "factoryName": "Green Recycling Factory",
  "quantity": 20.0,
  "pricePerUnit": 2.00,
  "totalAmount": 40.00,
  "stripePaymentIntentId": "pi_test123",
  "paymentStatus": "Pending",
  "purchaseDate": "2024-01-15T16:00:00Z",
  "createdAt": "2024-01-15T16:00:00Z",
  "marketplaceItem": {
    "id": "660e8400-e29b-41d4-a716-446655440000",
    "materialType": "Plastic",
    "quantity": 5.0,
    "unit": "Kg",
    "pricePerUnit": 2.00,
    "description": "Clean plastic bottles and containers"
  },
  "paymentResult": {
    "isSuccess": true,
    "message": "Payment processed successfully",
    "paymentIntentId": "pi_test123",
    "amount": 40.00,
    "currency": "usd",
    "status": "succeeded"
  }
}
```

**Validation Points**:
- ✅ Status code: 201 (Created)
- ✅ Purchase ID is generated
- ✅ Quantity is deducted from marketplace item
- ✅ Payment status is "Pending"
- ✅ Total amount is calculated correctly

## Role-Based Access Testing

### Test 1: User Permissions

**Test Cases**:
1. **User can access own pickup requests**
   - Endpoint: `GET /api/pickup-request/my-requests`
   - Expected: 200 OK

2. **User cannot access admin dashboard**
   - Endpoint: `GET /api/admin/dashboard`
   - Expected: 403 Forbidden

3. **User cannot approve pickup requests**
   - Endpoint: `PUT /api/pickup-request/{id}/approve`
   - Expected: 403 Forbidden

4. **User can browse marketplace**
   - Endpoint: `GET /api/marketplace`
   - Expected: 200 OK

### Test 2: Admin Permissions

**Test Cases**:
1. **Admin can access dashboard**
   - Endpoint: `GET /api/admin/dashboard`
   - Expected: 200 OK

2. **Admin can view pending requests**
   - Endpoint: `GET /api/pickup-request/pending`
   - Expected: 200 OK

3. **Admin can approve requests**
   - Endpoint: `PUT /api/pickup-request/{id}/approve`
   - Expected: 200 OK

4. **Admin can process payments**
   - Endpoint: `POST /api/payment/pickup/{id}`
   - Expected: 200 OK

### Test 3: Factory Permissions

**Test Cases**:
1. **Factory can view marketplace**
   - Endpoint: `GET /api/marketplace`
   - Expected: 200 OK

2. **Factory can purchase items**
   - Endpoint: `POST /api/marketplace/{id}/purchase`
   - Expected: 201 Created

3. **Factory can view own purchases**
   - Endpoint: `GET /api/purchase/my-purchases`
   - Expected: 200 OK

4. **Factory cannot approve requests**
   - Endpoint: `PUT /api/pickup-request/{id}/approve`
   - Expected: 403 Forbidden

## Error Handling Testing

### Test 1: Invalid Authentication

**Test Cases**:
1. **Missing token**
   - Headers: No Authorization header
   - Expected: 401 Unauthorized

2. **Invalid token format**
   - Headers: `Authorization: InvalidToken`
   - Expected: 401 Unauthorized

3. **Expired token**
   - Headers: `Authorization: Bearer <expired-token>`
   - Expected: 401 Unauthorized

### Test 2: Validation Errors

**Test Cases**:
1. **Invalid email format**
   ```json
   {
     "name": "Test User",
     "email": "invalid-email",
     "password": "TestPassword123!"
   }
   ```
   - Expected: 400 Bad Request with validation errors

2. **Missing required fields**
   ```json
   {
     "name": "Test User"
   }
   ```
   - Expected: 400 Bad Request with validation errors

3. **Invalid material type**
   ```json
   {
     "materialType": "InvalidType",
     "quantity": 25.0,
     "unit": "Kg"
   }
   ```
   - Expected: 400 Bad Request with validation errors

### Test 3: Business Logic Errors

**Test Cases**:
1. **Purchase more than available quantity**
   ```json
   {
     "marketplaceItemId": "item-id",
     "quantity": 1000.0,
     "pricePerUnit": 2.00
   }
   ```
   - Expected: 400 Bad Request with "Insufficient quantity" error

2. **Approve already approved request**
   - Endpoint: `PUT /api/pickup-request/{approved-id}/approve`
   - Expected: 400 Bad Request with "Request already approved" error

3. **Access non-existent resource**
   - Endpoint: `GET /api/pickup-request/non-existent-id`
   - Expected: 404 Not Found

## Performance Testing

### Test 1: Load Testing

**Tools**: Apache JMeter, Artillery, or k6

**Test Scenarios**:
1. **Concurrent user registration**: 100 users registering simultaneously
2. **Marketplace browsing**: 50 concurrent users browsing marketplace
3. **Payment processing**: 25 concurrent payment requests

**Expected Results**:
- Response time < 2 seconds
- Success rate > 95%
- No database deadlocks
- Proper error handling under load

### Test 2: Rate Limiting

**Test Cases**:
1. **Exceed rate limit**
   - Make 11 requests to auth endpoints in 1 minute
   - Expected: 429 Too Many Requests

2. **Rate limit headers**
   - Check response headers for rate limit information
   - Expected: `X-RateLimit-Limit`, `X-RateLimit-Remaining`, `X-RateLimit-Reset`

## Security Testing

### Test 1: JWT Token Security

**Test Cases**:
1. **Token tampering**
   - Modify JWT token payload
   - Expected: 401 Unauthorized

2. **Token signature verification**
   - Use token with invalid signature
   - Expected: 401 Unauthorized

3. **Token expiration**
   - Use expired token
   - Expected: 401 Unauthorized

### Test 2: Authorization Bypass

**Test Cases**:
1. **Role elevation**
   - Try to access admin endpoints with user token
   - Expected: 403 Forbidden

2. **Resource access control**
   - Try to access another user's data
   - Expected: 403 Forbidden

3. **Cross-role access**
   - Factory trying to approve pickup requests
   - Expected: 403 Forbidden

## Integration Testing

### Test 1: Database Integration

**Test Cases**:
1. **Data persistence**
   - Create pickup request and verify database record
   - Expected: Data correctly stored

2. **Transaction rollback**
   - Simulate payment failure and verify rollback
   - Expected: No partial data committed

3. **Data consistency**
   - Verify relationships between entities
   - Expected: Foreign key constraints maintained

### Test 2: External Service Integration

**Test Cases**:
1. **Stripe payment processing**
   - Test successful payment flow
   - Expected: Payment processed and confirmed

2. **Email notification sending**
   - Verify email notifications are sent
   - Expected: Emails delivered to correct addresses

3. **Webhook handling**
   - Test Stripe webhook processing
   - Expected: Webhook events processed correctly

## Automated Testing

### Unit Tests

**Framework**: xUnit, NUnit, or MSTest

**Test Categories**:
1. **Controller Tests**
   - Test all controller endpoints
   - Mock dependencies
   - Verify response formats

2. **Service Tests**
   - Test business logic
   - Mock repositories
   - Test error scenarios

3. **Repository Tests**
   - Test data access layer
   - Use in-memory database
   - Test CRUD operations

### Integration Tests

**Framework**: TestServer or WebApplicationFactory

**Test Categories**:
1. **API Integration Tests**
   - Test complete request/response cycle
   - Use test database
   - Verify end-to-end functionality

2. **Database Integration Tests**
   - Test actual database operations
   - Use test database
   - Clean up after tests

### End-to-End Tests

**Framework**: Selenium, Playwright, or Cypress

**Test Scenarios**:
1. **Complete user workflow**
   - Registration → Login → Create Request → Admin Approval → Payment
   - Expected: All steps complete successfully

2. **Complete factory workflow**
   - Registration → Login → Browse Marketplace → Purchase → Confirm Payment
   - Expected: All steps complete successfully

## Test Data Management

### Test Data Setup

**Required Test Data**:
1. **Users**
   - Test user (role: User)
   - Test admin (role: Admin)
   - Test factory (role: Factory)

2. **Pickup Requests**
   - Pending requests
   - Approved requests
   - Rejected requests

3. **Marketplace Items**
   - Available items
   - Sold items
   - Different material types

### Test Data Cleanup

**Cleanup Strategy**:
1. **Database cleanup**
   - Delete test data after each test
   - Use database transactions
   - Reset auto-increment counters

2. **File cleanup**
   - Remove uploaded test images
   - Clean temporary files
   - Reset file storage

## Monitoring and Logging

### Test Monitoring

**Monitor During Testing**:
1. **Application logs**
   - Error logs
   - Performance logs
   - Security logs

2. **Database metrics**
   - Query performance
   - Connection pool usage
   - Transaction rates

3. **External service metrics**
   - Stripe API calls
   - Email delivery rates
   - Response times

### Test Reporting

**Report Metrics**:
1. **Test coverage**
   - API endpoint coverage
   - Business logic coverage
   - Error scenario coverage

2. **Performance metrics**
   - Response times
   - Throughput
   - Error rates

3. **Security metrics**
   - Authentication success/failure rates
   - Authorization bypass attempts
   - Rate limiting effectiveness

## Troubleshooting Guide

### Common Issues

1. **Authentication Errors**
   - **Issue**: 401 Unauthorized
   - **Solution**: Check token format and expiration
   - **Debug**: Verify JWT token structure

2. **Authorization Errors**
   - **Issue**: 403 Forbidden
   - **Solution**: Check user role and permissions
   - **Debug**: Verify role claims in JWT token

3. **Validation Errors**
   - **Issue**: 400 Bad Request
   - **Solution**: Check request body format
   - **Debug**: Review validation error messages

4. **Database Errors**
   - **Issue**: 500 Internal Server Error
   - **Solution**: Check database connection and constraints
   - **Debug**: Review application logs

### Debug Tools

1. **Postman Console**
   - View request/response details
   - Check headers and body
   - Monitor response times

2. **Application Logs**
   - Review error messages
   - Check stack traces
   - Monitor performance

3. **Database Logs**
   - Review SQL queries
   - Check constraint violations
   - Monitor transaction logs

## Best Practices

### Testing Best Practices

1. **Test Isolation**
   - Each test should be independent
   - Clean up test data
   - Use unique test identifiers

2. **Test Data Management**
   - Use realistic test data
   - Avoid hardcoded values
   - Maintain test data consistency

3. **Error Testing**
   - Test both success and failure scenarios
   - Verify error messages
   - Test edge cases

4. **Performance Testing**
   - Test under realistic load
   - Monitor resource usage
   - Set performance benchmarks

### Security Best Practices

1. **Authentication Testing**
   - Test all authentication scenarios
   - Verify token security
   - Test session management

2. **Authorization Testing**
   - Test role-based access
   - Verify resource isolation
   - Test privilege escalation attempts

3. **Input Validation Testing**
   - Test malicious input
   - Verify sanitization
   - Test injection attacks

## Conclusion

This testing guide provides a comprehensive approach to testing the NADAFA API. Follow these guidelines to ensure:

- ✅ All endpoints work correctly
- ✅ Security measures are effective
- ✅ Performance meets requirements
- ✅ Error handling is robust
- ✅ Integration with external services works properly

Regular testing using this guide will help maintain API quality and reliability.
