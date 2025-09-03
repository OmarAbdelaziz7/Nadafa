# NADAFA Recycling Platform - Services Implementation

## Overview

This document provides comprehensive documentation for the NADAFA recycling platform's core business services and repositories implementation. The platform follows a clean architecture pattern with clear separation of concerns.

## Architecture

### Layer Structure
```
Presentation (Controllers)
    ↓
Application (Services + DTOs)
    ↓
Infrastructure (Repositories + DbContext)
    ↓
Domain (Entities + Enums)
```

### Core Workflow
1. **User creates pickup request** → Admin approves → Payment processed → Items published to marketplace → Factory purchases

### Key Components

#### Repositories (Infrastructure Layer)
- `IPickupRequestRepository` / `PickupRequestRepository`
- `IMarketplaceRepository` / `MarketplaceRepository`
- `IPurchaseRepository` / `PurchaseRepository`
- `INotificationRepository` / `NotificationRepository`

#### Services (Application Layer)
- `IPickupRequestService` / `PickupRequestService`
- `IMarketplaceService` / `MarketplaceService`
- `IPurchaseService` / `PurchaseService`
- `INotificationService` / `NotificationService`

#### DTOs (Application Layer)
- `CreatePickupRequestDto`, `PickupRequestResponseDto`
- `MarketplaceItemDto`, `PaginatedMarketplaceItemsDto`
- `CreatePurchaseDto`, `PurchaseResponseDto`
- `ApprovePickupRequestDto`, `RejectPickupRequestDto`

## API Endpoints & Examples

### 1. Pickup Request Management

#### Create Pickup Request
```http
POST /api/pickup-requests
Content-Type: application/json
Authorization: Bearer {token}

{
  "materialType": 1,
  "quantity": 50.5,
  "unit": 1,
  "proposedPricePerUnit": 2.50,
  "description": "Clean paper waste from office",
  "imageUrls": [
    "https://example.com/image1.jpg",
    "https://example.com/image2.jpg"
  ]
}
```

**Response:**
```json
{
  "id": "550e8400-e29b-41d4-a716-446655440000",
  "userId": 1,
  "userName": "John Doe",
  "materialType": 1,
  "quantity": 50.5,
  "unit": 1,
  "proposedPricePerUnit": 2.50,
  "totalEstimatedPrice": 126.25,
  "description": "Clean paper waste from office",
  "imageUrls": ["https://example.com/image1.jpg", "https://example.com/image2.jpg"],
  "status": 1,
  "requestDate": "2024-01-15T10:30:00Z",
  "approvedDate": null,
  "pickupDate": null,
  "adminId": null,
  "adminName": null,
  "adminNotes": null,
  "createdAt": "2024-01-15T10:30:00Z",
  "updatedAt": "2024-01-15T10:30:00Z",
  "marketplaceItem": null
}
```

#### Get User Requests (Paginated)
```http
GET /api/pickup-requests/user?page=1&pageSize=10
Authorization: Bearer {token}
```

**Response:**
```json
{
  "items": [
    {
      "id": "550e8400-e29b-41d4-a716-446655440000",
      "userId": 1,
      "userName": "John Doe",
      "materialType": 1,
      "quantity": 50.5,
      "unit": 1,
      "proposedPricePerUnit": 2.50,
      "totalEstimatedPrice": 126.25,
      "description": "Clean paper waste from office",
      "status": 1,
      "requestDate": "2024-01-15T10:30:00Z",
      "createdAt": "2024-01-15T10:30:00Z"
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

#### Approve Pickup Request (Admin)
```http
PUT /api/pickup-requests/{id}/approve
Content-Type: application/json
Authorization: Bearer {admin-token}

{
  "notes": "Approved after inspection. Good quality paper waste."
}
```

#### Reject Pickup Request (Admin)
```http
PUT /api/pickup-requests/{id}/reject
Content-Type: application/json
Authorization: Bearer {admin-token}

{
  "notes": "Rejected due to contamination. Please clean before resubmitting."
}
```

### 2. Marketplace Management

#### Get Available Items
```http
GET /api/marketplace/items?filter=1&page=1&pageSize=10
```

**Response:**
```json
{
  "items": [
    {
      "id": "660e8400-e29b-41d4-a716-446655440000",
      "pickupRequestId": "550e8400-e29b-41d4-a716-446655440000",
      "userId": 1,
      "userName": "John Doe",
      "materialType": 1,
      "quantity": 50.5,
      "unit": 1,
      "pricePerUnit": 2.50,
      "totalPrice": 126.25,
      "description": "Clean paper waste from office",
      "imageUrls": ["https://example.com/image1.jpg"],
      "isAvailable": true,
      "publishedAt": "2024-01-15T11:00:00Z",
      "createdAt": "2024-01-15T11:00:00Z",
      "purchase": null
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

#### Search Items
```http
GET /api/marketplace/search?searchTerm=paper&page=1&pageSize=10
```

#### Get Item Details
```http
GET /api/marketplace/items/{id}
```

### 3. Purchase Management

#### Create Purchase (Factory)
```http
POST /api/purchases
Content-Type: application/json
Authorization: Bearer {factory-token}

{
  "marketplaceItemId": "660e8400-e29b-41d4-a716-446655440000",
  "quantity": 50.5,
  "pricePerUnit": 2.50,
  "stripePaymentIntentId": "pi_1234567890abcdef"
}
```

**Response:**
```json
{
  "id": "770e8400-e29b-41d4-a716-446655440000",
  "marketplaceItemId": "660e8400-e29b-41d4-a716-446655440000",
  "factoryId": 1,
  "factoryName": "Green Recycling Factory",
  "quantity": 50.5,
  "pricePerUnit": 2.50,
  "totalAmount": 126.25,
  "stripePaymentIntentId": "pi_1234567890abcdef",
  "paymentStatus": 1,
  "purchaseDate": "2024-01-15T12:00:00Z",
  "createdAt": "2024-01-15T12:00:00Z",
  "marketplaceItem": {
    "id": "660e8400-e29b-41d4-a716-446655440000",
    "userId": 1,
    "userName": "John Doe",
    "materialType": 1,
    "quantity": 50.5,
    "unit": 1,
    "pricePerUnit": 2.50,
    "totalPrice": 126.25,
    "description": "Clean paper waste from office",
    "isAvailable": false
  }
}
```

#### Get Factory Purchases
```http
GET /api/purchases/factory?page=1&pageSize=10
Authorization: Bearer {factory-token}
```

#### Update Payment Status
```http
PUT /api/purchases/{id}/payment-status
Content-Type: application/json
Authorization: Bearer {admin-token}

{
  "status": 2
}
```

## Manual Testing Scenarios

### 1. Pickup Request Workflow

#### Test Case 1: Create Pickup Request
1. **Prerequisites**: User is authenticated
2. **Steps**:
   - Send POST request to `/api/pickup-requests`
   - Include valid pickup request data
3. **Expected Result**:
   - Status 201 Created
   - Pickup request created with Pending status
   - Request stored in database

#### Test Case 2: Admin Approves Request
1. **Prerequisites**: Admin is authenticated, pickup request exists with Pending status
2. **Steps**:
   - Send PUT request to `/api/pickup-requests/{id}/approve`
   - Include approval notes
3. **Expected Result**:
   - Status 200 OK
   - Request status changed to Approved
   - Notification created for user
   - Admin notes saved

#### Test Case 3: Admin Rejects Request
1. **Prerequisites**: Admin is authenticated, pickup request exists with Pending status
2. **Steps**:
   - Send PUT request to `/api/pickup-requests/{id}/reject`
   - Include rejection notes
3. **Expected Result**:
   - Status 200 OK
   - Request status changed to Rejected
   - Notification created for user
   - Admin notes saved

### 2. Marketplace Workflow

#### Test Case 4: Create Marketplace Item
1. **Prerequisites**: Approved pickup request exists
2. **Steps**:
   - Call `CreateMarketplaceItemAsync` service method
3. **Expected Result**:
   - Marketplace item created
   - Pickup request status changed to Published
   - Item marked as available

#### Test Case 5: Search Marketplace Items
1. **Prerequisites**: Marketplace items exist
2. **Steps**:
   - Send GET request to `/api/marketplace/search?searchTerm=paper`
3. **Expected Result**:
   - Status 200 OK
   - Paginated results returned
   - Only available items included

### 3. Purchase Workflow

#### Test Case 6: Factory Purchases Item
1. **Prerequisites**: Factory is authenticated, marketplace item is available
2. **Steps**:
   - Send POST request to `/api/purchases`
   - Include valid purchase data
3. **Expected Result**:
   - Status 201 Created
   - Purchase record created
   - Marketplace item marked as unavailable
   - Notification sent to seller

#### Test Case 7: Update Payment Status
1. **Prerequisites**: Purchase exists with Pending payment status
2. **Steps**:
   - Send PUT request to `/api/purchases/{id}/payment-status`
   - Set status to Completed
3. **Expected Result**:
   - Status 200 OK
   - Payment status updated
   - Notification sent to seller

## Database Queries for Verification

### 1. Verify Pickup Request Creation
```sql
SELECT 
    pr.Id,
    pr.UserId,
    u.Name as UserName,
    pr.MaterialType,
    pr.Quantity,
    pr.Unit,
    pr.ProposedPricePerUnit,
    pr.Status,
    pr.RequestDate,
    pr.CreatedAt
FROM PickupRequests pr
JOIN Users u ON pr.UserId = u.Id
WHERE pr.Id = '550e8400-e29b-41d4-a716-446655440000';
```

### 2. Verify Marketplace Item Creation
```sql
SELECT 
    mi.Id,
    mi.PickupRequestId,
    mi.UserId,
    u.Name as UserName,
    mi.MaterialType,
    mi.Quantity,
    mi.PricePerUnit,
    mi.IsAvailable,
    mi.PublishedAt,
    pr.Status as PickupRequestStatus
FROM MarketplaceItems mi
JOIN Users u ON mi.UserId = u.Id
JOIN PickupRequests pr ON mi.PickupRequestId = pr.Id
WHERE mi.Id = '660e8400-e29b-41d4-a716-446655440000';
```

### 3. Verify Purchase Creation
```sql
SELECT 
    p.Id,
    p.MarketplaceItemId,
    p.FactoryId,
    f.Name as FactoryName,
    p.Quantity,
    p.PricePerUnit,
    p.PaymentStatus,
    p.PurchaseDate,
    mi.IsAvailable as ItemAvailable
FROM Purchases p
JOIN Factories f ON p.FactoryId = f.Id
JOIN MarketplaceItems mi ON p.MarketplaceItemId = mi.Id
WHERE p.Id = '770e8400-e29b-41d4-a716-446655440000';
```

### 4. Verify Notifications
```sql
SELECT 
    n.Id,
    n.UserId,
    u.Name as UserName,
    n.Title,
    n.Message,
    n.NotificationType,
    n.IsRead,
    n.CreatedAt
FROM Notifications n
JOIN Users u ON n.UserId = u.Id
WHERE n.UserId = 1
ORDER BY n.CreatedAt DESC;
```

### 5. Complete Workflow Verification
```sql
-- Get complete workflow for a specific pickup request
SELECT 
    pr.Id as PickupRequestId,
    pr.Status as PickupStatus,
    pr.RequestDate,
    pr.ApprovedDate,
    mi.Id as MarketplaceItemId,
    mi.IsAvailable as ItemAvailable,
    mi.PublishedAt,
    p.Id as PurchaseId,
    p.PaymentStatus,
    p.PurchaseDate,
    n.Title as NotificationTitle,
    n.NotificationType
FROM PickupRequests pr
LEFT JOIN MarketplaceItems mi ON pr.Id = mi.PickupRequestId
LEFT JOIN Purchases p ON mi.Id = p.MarketplaceItemId
LEFT JOIN Notifications n ON pr.UserId = n.UserId
WHERE pr.Id = '550e8400-e29b-41d4-a716-446655440000'
ORDER BY pr.RequestDate, mi.PublishedAt, p.PurchaseDate, n.CreatedAt;
```

## Error Handling Patterns

### 1. Repository Layer Errors
- **Entity Not Found**: Return null, let service layer handle
- **Database Connection**: Let exception bubble up to global handler
- **Validation Errors**: Use domain validation attributes

### 2. Service Layer Errors
- **Business Rule Violations**: Throw `InvalidOperationException`
- **Invalid Input**: Throw `ArgumentException`
- **Entity Not Found**: Throw `ArgumentException` with descriptive message

### 3. Controller Layer Errors
```csharp
try
{
    var result = await _service.MethodAsync(dto);
    return Ok(result);
}
catch (ArgumentException ex)
{
    return BadRequest(new { error = ex.Message });
}
catch (InvalidOperationException ex)
{
    return BadRequest(new { error = ex.Message });
}
catch (Exception ex)
{
    return StatusCode(500, new { error = "An unexpected error occurred" });
}
```

## Performance Considerations

### 1. Database Optimization
- Use appropriate indexes on frequently queried columns
- Implement pagination for large datasets
- Use Include() for related data to avoid N+1 queries

### 2. Caching Strategy
- Cache frequently accessed marketplace items
- Cache user notifications
- Implement Redis for session management

### 3. Async/Await Pattern
- All repository methods are async
- All service methods are async
- Proper exception handling in async contexts

## Security Considerations

### 1. Authentication & Authorization
- JWT token validation for all endpoints
- Role-based access control (User, Admin, Factory)
- User can only access their own data

### 2. Input Validation
- DTO validation using Data Annotations
- Business rule validation in service layer
- SQL injection prevention through Entity Framework

### 3. Data Protection
- Sensitive data encryption
- Audit logging for admin actions
- Secure payment processing with Stripe

## Deployment Checklist

### 1. Database Migration
```bash
dotnet ef database update
```

### 2. Environment Configuration
- Set connection strings
- Configure JWT secrets
- Set Stripe API keys

### 3. Service Registration
- Verify all services registered in DI container
- Test dependency injection
- Validate service lifetimes

### 4. Health Checks
- Database connectivity
- External service dependencies
- Application startup validation

## Monitoring & Logging

### 1. Application Logs
- Request/response logging
- Error logging with stack traces
- Performance metrics

### 2. Business Metrics
- Pickup request approval rate
- Marketplace item conversion rate
- Payment success rate

### 3. Alerting
- Database connection failures
- High error rates
- Payment processing failures

This implementation provides a robust foundation for the NADAFA recycling platform with clean architecture, comprehensive error handling, and thorough testing scenarios.
