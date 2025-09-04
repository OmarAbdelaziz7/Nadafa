# NADAFA Recycling Platform - API Documentation

## Overview

The NADAFA recycling platform provides a comprehensive REST API for managing the complete recycling workflow from pickup requests to marketplace transactions. The API supports three user roles: **User**, **Admin**, and **Factory**, each with specific permissions and access levels.

## Base URL

```
https://api.nadafa.com/v1
```

## Authentication

All API endpoints (except registration and login) require JWT Bearer token authentication.

### JWT Token Format

```
Authorization: Bearer <your-jwt-token>
```

### Token Claims

- `sub` (Subject): User ID
- `email`: User email address
- `name`: User name
- `role`: User role (User, Admin, Factory)
- `exp`: Expiration timestamp
- `iat`: Issued at timestamp

## Role-Based Access Control

### User Role
- Create and manage pickup requests
- Browse marketplace items
- View own notifications
- Manage profile

### Admin Role
- Approve/reject pickup requests
- Process payments
- Manage marketplace
- View system statistics
- Export data

### Factory Role
- Purchase marketplace items
- Manage purchases
- Confirm payments
- View own notifications

## API Endpoints

---

## Authentication Endpoints

### 1. User Registration

**POST** `/api/auth/register/user`

Register a new user account.

**Request Body:**
```json
{
  "name": "John Doe",
  "email": "john@example.com",
  "password": "SecurePassword123!",
  "phone": "+1234567890",
  "address": "123 Main St, City, Country"
}
```

**Response:**
```json
{
  "isSuccess": true,
  "message": "User registered successfully",
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "user": {
    "id": 1,
    "name": "John Doe",
    "email": "john@example.com",
    "role": "User"
  }
}
```

### 2. Factory Registration

**POST** `/api/auth/register/factory`

Register a new factory account.

**Request Body:**
```json
{
  "name": "Green Recycling Factory",
  "email": "factory@example.com",
  "password": "SecurePassword123!",
  "phone": "+1234567890",
  "address": "456 Industrial Ave, City, Country",
  "businessLicense": "LIC123456",
  "capacity": 1000
}
```

**Response:**
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

### 3. Login

**POST** `/api/auth/login`

Authenticate user or factory.

**Request Body:**
```json
{
  "email": "john@example.com",
  "password": "SecurePassword123!"
}
```

**Response:**
```json
{
  "isSuccess": true,
  "message": "Login successful",
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "user": {
    "id": 1,
    "name": "John Doe",
    "email": "john@example.com",
    "role": "User"
  }
}
```

### 4. Get Current User

**GET** `/api/auth/me`

Get current user information.

**Headers:**
```
Authorization: Bearer <token>
```

**Response:**
```json
{
  "userId": "1",
  "email": "john@example.com",
  "name": "John Doe",
  "role": "User",
  "claims": [
    {
      "type": "sub",
      "value": "1"
    },
    {
      "type": "email",
      "value": "john@example.com"
    }
  ]
}
```

---

## Pickup Request Endpoints

### 1. Create Pickup Request

**POST** `/api/pickup-request`

Create a new pickup request (User only).

**Headers:**
```
Authorization: Bearer <user-token>
```

**Request Body:**
```json
{
  "materialType": "Plastic",
  "quantity": 50.5,
  "unit": "Kg",
  "proposedPricePerUnit": 2.50,
  "description": "Clean plastic bottles and containers",
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
  "materialType": "Plastic",
  "quantity": 50.5,
  "unit": "Kg",
  "proposedPricePerUnit": 2.50,
  "totalEstimatedPrice": 126.25,
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

### 2. Get User's Requests

**GET** `/api/pickup-request/my-requests?page=1&pageSize=10`

Get user's pickup requests with pagination (User only).

**Headers:**
```
Authorization: Bearer <user-token>
```

**Response:**
```json
{
  "items": [
    {
      "id": "550e8400-e29b-41d4-a716-446655440000",
      "userId": 1,
      "userName": "John Doe",
      "materialType": "Plastic",
      "quantity": 50.5,
      "unit": "Kg",
      "proposedPricePerUnit": 2.50,
      "totalEstimatedPrice": 126.25,
      "description": "Clean plastic bottles and containers",
      "status": "Pending",
      "requestDate": "2024-01-15T10:30:00Z"
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

### 3. Get Pending Requests

**GET** `/api/pickup-request/pending?page=1&pageSize=10`

Get pending pickup requests (Admin only).

**Headers:**
```
Authorization: Bearer <admin-token>
```

**Response:**
```json
{
  "items": [
    {
      "id": "550e8400-e29b-41d4-a716-446655440000",
      "userId": 1,
      "userName": "John Doe",
      "materialType": "Plastic",
      "quantity": 50.5,
      "unit": "Kg",
      "proposedPricePerUnit": 2.50,
      "totalEstimatedPrice": 126.25,
      "description": "Clean plastic bottles and containers",
      "status": "Pending",
      "requestDate": "2024-01-15T10:30:00Z"
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

### 4. Get Request Details

**GET** `/api/pickup-request/{id}`

Get pickup request details (User: own requests, Admin: all).

**Headers:**
```
Authorization: Bearer <token>
```

**Response:**
```json
{
  "id": "550e8400-e29b-41d4-a716-446655440000",
  "userId": 1,
  "userName": "John Doe",
  "materialType": "Plastic",
  "quantity": 50.5,
  "unit": "Kg",
  "proposedPricePerUnit": 2.50,
  "totalEstimatedPrice": 126.25,
  "description": "Clean plastic bottles and containers",
  "imageUrls": [
    "https://example.com/image1.jpg",
    "https://example.com/image2.jpg"
  ],
  "status": "Approved",
  "requestDate": "2024-01-15T10:30:00Z",
  "approvedAt": "2024-01-15T14:30:00Z",
  "approvedBy": "Admin User",
  "paymentAmount": 126.25
}
```

### 5. Approve Request

**PUT** `/api/pickup-request/{id}/approve`

Approve pickup request (Admin only).

**Headers:**
```
Authorization: Bearer <admin-token>
```

**Request Body:**
```json
{
  "approvedPricePerUnit": 2.50,
  "notes": "Request approved after inspection"
}
```

**Response:**
```json
{
  "id": "550e8400-e29b-41d4-a716-446655440000",
  "userId": 1,
  "userName": "John Doe",
  "materialType": "Plastic",
  "quantity": 50.5,
  "unit": "Kg",
  "proposedPricePerUnit": 2.50,
  "approvedPricePerUnit": 2.50,
  "totalEstimatedPrice": 126.25,
  "description": "Clean plastic bottles and containers",
  "status": "Approved",
  "requestDate": "2024-01-15T10:30:00Z",
  "approvedAt": "2024-01-15T14:30:00Z",
  "approvedBy": "Admin User"
}
```

### 6. Reject Request

**PUT** `/api/pickup-request/{id}/reject`

Reject pickup request (Admin only).

**Headers:**
```
Authorization: Bearer <admin-token>
```

**Request Body:**
```json
{
  "reason": "Materials not properly sorted",
  "notes": "Please separate different types of plastic"
}
```

**Response:**
```json
{
  "id": "550e8400-e29b-41d4-a716-446655440000",
  "userId": 1,
  "userName": "John Doe",
  "materialType": "Plastic",
  "quantity": 50.5,
  "unit": "Kg",
  "proposedPricePerUnit": 2.50,
  "totalEstimatedPrice": 126.25,
  "description": "Clean plastic bottles and containers",
  "status": "Rejected",
  "requestDate": "2024-01-15T10:30:00Z",
  "rejectedAt": "2024-01-15T14:30:00Z",
  "rejectedBy": "Admin User",
  "rejectionReason": "Materials not properly sorted"
}
```

---

## Marketplace Endpoints

### 1. Get Available Items

**GET** `/api/marketplace?materialType=Plastic&page=1&pageSize=10`

Get available marketplace items with filters (All authenticated users).

**Headers:**
```
Authorization: Bearer <token>
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
      "materialType": "Plastic",
      "quantity": 50.5,
      "unit": "Kg",
      "pricePerUnit": 2.50,
      "totalPrice": 126.25,
      "description": "Clean plastic bottles and containers",
      "imageUrls": [
        "https://example.com/image1.jpg",
        "https://example.com/image2.jpg"
      ],
      "isAvailable": true,
      "publishedAt": "2024-01-15T15:00:00Z",
      "createdAt": "2024-01-15T15:00:00Z"
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

### 2. Get Item Details

**GET** `/api/marketplace/{id}`

Get marketplace item details (All authenticated users).

**Headers:**
```
Authorization: Bearer <token>
```

**Response:**
```json
{
  "id": "660e8400-e29b-41d4-a716-446655440000",
  "pickupRequestId": "550e8400-e29b-41d4-a716-446655440000",
  "userId": 1,
  "userName": "John Doe",
  "materialType": "Plastic",
  "quantity": 50.5,
  "unit": "Kg",
  "pricePerUnit": 2.50,
  "totalPrice": 126.25,
  "description": "Clean plastic bottles and containers",
  "imageUrls": [
    "https://example.com/image1.jpg",
    "https://example.com/image2.jpg"
  ],
  "isAvailable": true,
  "publishedAt": "2024-01-15T15:00:00Z",
  "createdAt": "2024-01-15T15:00:00Z",
  "purchase": null
}
```

### 3. Search Items

**GET** `/api/marketplace/search?searchTerm=plastic&page=1&pageSize=10`

Search marketplace items (All authenticated users).

**Headers:**
```
Authorization: Bearer <token>
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
      "materialType": "Plastic",
      "quantity": 50.5,
      "unit": "Kg",
      "pricePerUnit": 2.50,
      "totalPrice": 126.25,
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

### 4. Purchase Item

**POST** `/api/marketplace/{id}/purchase`

Purchase marketplace item (Factory only).

**Headers:**
```
Authorization: Bearer <factory-token>
```

**Request Body:**
```json
{
  "marketplaceItemId": "660e8400-e29b-41d4-a716-446655440000",
  "quantity": 25.0,
  "pricePerUnit": 2.50,
  "stripePaymentIntentId": "pi_1234567890"
}
```

**Response:**
```json
{
  "id": "770e8400-e29b-41d4-a716-446655440000",
  "marketplaceItemId": "660e8400-e29b-41d4-a716-446655440000",
  "factoryId": 2,
  "factoryName": "Green Recycling Factory",
  "quantity": 25.0,
  "pricePerUnit": 2.50,
  "totalAmount": 62.50,
  "stripePaymentIntentId": "pi_1234567890",
  "paymentStatus": "Pending",
  "purchaseDate": "2024-01-15T16:00:00Z",
  "createdAt": "2024-01-15T16:00:00Z",
  "marketplaceItem": {
    "id": "660e8400-e29b-41d4-a716-446655440000",
    "materialType": "Plastic",
    "quantity": 25.5,
    "unit": "Kg",
    "pricePerUnit": 2.50,
    "description": "Clean plastic bottles and containers"
  },
  "paymentResult": {
    "isSuccess": true,
    "message": "Payment processed successfully",
    "paymentIntentId": "pi_1234567890",
    "amount": 62.50,
    "currency": "usd",
    "status": "succeeded"
  }
}
```

---

## Purchase Endpoints

### 1. Get Factory's Purchases

**GET** `/api/purchase/my-purchases?page=1&pageSize=10`

Get factory's purchases (Factory only).

**Headers:**
```
Authorization: Bearer <factory-token>
```

**Response:**
```json
{
  "items": [
    {
      "id": "770e8400-e29b-41d4-a716-446655440000",
      "marketplaceItemId": "660e8400-e29b-41d4-a716-446655440000",
      "factoryId": 2,
      "factoryName": "Green Recycling Factory",
      "quantity": 25.0,
      "pricePerUnit": 2.50,
      "totalAmount": 62.50,
      "stripePaymentIntentId": "pi_1234567890",
      "paymentStatus": "Completed",
      "purchaseDate": "2024-01-15T16:00:00Z",
      "createdAt": "2024-01-15T16:00:00Z"
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

### 2. Get Purchase Details

**GET** `/api/purchase/{id}`

Get purchase details (Factory: own purchases, Admin: all).

**Headers:**
```
Authorization: Bearer <token>
```

**Response:**
```json
{
  "id": "770e8400-e29b-41d4-a716-446655440000",
  "marketplaceItemId": "660e8400-e29b-41d4-a716-446655440000",
  "factoryId": 2,
  "factoryName": "Green Recycling Factory",
  "quantity": 25.0,
  "pricePerUnit": 2.50,
  "totalAmount": 62.50,
  "stripePaymentIntentId": "pi_1234567890",
  "paymentStatus": "Completed",
  "purchaseDate": "2024-01-15T16:00:00Z",
  "createdAt": "2024-01-15T16:00:00Z",
  "marketplaceItem": {
    "id": "660e8400-e29b-41d4-a716-446655440000",
    "materialType": "Plastic",
    "quantity": 25.5,
    "unit": "Kg",
    "pricePerUnit": 2.50,
    "description": "Clean plastic bottles and containers"
  }
}
```

### 3. Confirm Payment

**POST** `/api/purchase/{id}/confirm-payment`

Confirm payment for purchase (Factory only).

**Headers:**
```
Authorization: Bearer <factory-token>
```

**Request Body:**
```json
{
  "status": "Completed"
}
```

**Response:**
```json
{
  "id": "770e8400-e29b-41d4-a716-446655440000",
  "marketplaceItemId": "660e8400-e29b-41d4-a716-446655440000",
  "factoryId": 2,
  "factoryName": "Green Recycling Factory",
  "quantity": 25.0,
  "pricePerUnit": 2.50,
  "totalAmount": 62.50,
  "stripePaymentIntentId": "pi_1234567890",
  "paymentStatus": "Completed",
  "purchaseDate": "2024-01-15T16:00:00Z",
  "updatedAt": "2024-01-15T16:30:00Z"
}
```

---

## Admin Endpoints

### 1. Get Dashboard Stats

**GET** `/api/admin/dashboard`

Get admin dashboard statistics (Admin only).

**Headers:**
```
Authorization: Bearer <admin-token>
```

**Response:**
```json
{
  "totalUsers": 150,
  "totalFactories": 25,
  "pendingPickupRequests": 12,
  "approvedPickupRequests": 89,
  "totalMarketplaceItems": 67,
  "availableMarketplaceItems": 45,
  "totalPurchases": 34,
  "totalRevenue": 12500.75,
  "recentActivity": [
    {
      "type": "PickupRequest",
      "action": "Created",
      "userId": 1,
      "userName": "John Doe",
      "timestamp": "2024-01-15T10:30:00Z"
    },
    {
      "type": "Purchase",
      "action": "Completed",
      "factoryId": 2,
      "factoryName": "Green Recycling Factory",
      "amount": 62.50,
      "timestamp": "2024-01-15T16:30:00Z"
    }
  ]
}
```

### 2. Get All Requests

**GET** `/api/admin/requests?status=Pending&materialType=Plastic&page=1&pageSize=10`

Get all pickup requests with filters (Admin only).

**Headers:**
```
Authorization: Bearer <admin-token>
```

**Response:**
```json
{
  "items": [
    {
      "id": "550e8400-e29b-41d4-a716-446655440000",
      "userId": 1,
      "userName": "John Doe",
      "materialType": "Plastic",
      "quantity": 50.5,
      "unit": "Kg",
      "proposedPricePerUnit": 2.50,
      "totalEstimatedPrice": 126.25,
      "description": "Clean plastic bottles and containers",
      "status": "Pending",
      "requestDate": "2024-01-15T10:30:00Z"
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

### 3. Get Payment History

**GET** `/api/admin/payments?fromDate=2024-01-01&toDate=2024-01-31&page=1&pageSize=10`

Get payment history (Admin only).

**Headers:**
```
Authorization: Bearer <admin-token>
```

**Response:**
```json
{
  "payments": [
    {
      "id": "880e8400-e29b-41d4-a716-446655440000",
      "type": "PickupPayment",
      "amount": 126.25,
      "currency": "usd",
      "status": "Completed",
      "userId": 1,
      "userName": "John Doe",
      "pickupRequestId": "550e8400-e29b-41d4-a716-446655440000",
      "processedAt": "2024-01-15T14:30:00Z"
    }
  ],
  "totalCount": 1,
  "page": 1,
  "pageSize": 10,
  "totalPages": 1,
  "hasNextPage": false,
  "hasPreviousPage": false,
  "totalAmount": 126.25
}
```

### 4. Publish to Marketplace

**POST** `/api/admin/publish-to-marketplace`

Manually publish approved items to marketplace (Admin only).

**Headers:**
```
Authorization: Bearer <admin-token>
```

**Request Body:**
```json
{
  "pickupRequestId": "550e8400-e29b-41d4-a716-446655440000"
}
```

**Response:**
```json
{
  "id": "660e8400-e29b-41d4-a716-446655440000",
  "pickupRequestId": "550e8400-e29b-41d4-a716-446655440000",
  "userId": 1,
  "userName": "John Doe",
  "materialType": "Plastic",
  "quantity": 50.5,
  "unit": "Kg",
  "pricePerUnit": 2.50,
  "totalPrice": 126.25,
  "description": "Clean plastic bottles and containers",
  "imageUrls": [
    "https://example.com/image1.jpg",
    "https://example.com/image2.jpg"
  ],
  "isAvailable": true,
  "publishedAt": "2024-01-15T15:00:00Z",
  "createdAt": "2024-01-15T15:00:00Z"
}
```

---

## Notification Endpoints

### 1. Get Notifications

**GET** `/api/notification?unreadOnly=false&page=1&pageSize=10`

Get user notifications (All authenticated users).

**Headers:**
```
Authorization: Bearer <token>
```

**Response:**
```json
{
  "items": [
    {
      "id": "990e8400-e29b-41d4-a716-446655440000",
      "userId": 1,
      "userName": "John Doe",
      "type": "PickupApproved",
      "title": "Pickup Request Approved",
      "message": "Your pickup request for 50.5 Kg of Plastic has been approved. Payment of $126.25 has been processed.",
      "isRead": false,
      "createdAt": "2024-01-15T14:30:00Z",
      "readAt": null
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

### 2. Mark as Read

**PUT** `/api/notification/{id}/read`

Mark notification as read (All authenticated users).

**Headers:**
```
Authorization: Bearer <token>
```

**Response:**
```json
{
  "id": "990e8400-e29b-41d4-a716-446655440000",
  "userId": 1,
  "userName": "John Doe",
  "type": "PickupApproved",
  "title": "Pickup Request Approved",
  "message": "Your pickup request for 50.5 Kg of Plastic has been approved. Payment of $126.25 has been processed.",
  "isRead": true,
  "createdAt": "2024-01-15T14:30:00Z",
  "readAt": "2024-01-15T16:00:00Z"
}
```

### 3. Mark All as Read

**PUT** `/api/notification/mark-all-read`

Mark all notifications as read (All authenticated users).

**Headers:**
```
Authorization: Bearer <token>
```

**Response:**
```json
{
  "message": "5 notifications marked as read",
  "count": 5
}
```

### 4. Get Unread Count

**GET** `/api/notification/unread-count`

Get unread notification count (All authenticated users).

**Headers:**
```
Authorization: Bearer <token>
```

**Response:**
```json
{
  "count": 3
}
```

---

## Payment Endpoints

### 1. Process Pickup Payment

**POST** `/api/payment/pickup/{pickupRequestId}`

Process payment for approved pickup request (Admin pays user).

**Headers:**
```
Authorization: Bearer <admin-token>
```

**Request Body:**
```json
{
  "amount": 126.25
}
```

**Response:**
```json
{
  "isSuccess": true,
  "message": "Payment processed successfully",
  "paymentIntentId": "pi_1234567890",
  "clientSecret": "pi_1234567890_secret_abc123",
  "amount": 126.25,
  "currency": "usd",
  "status": "succeeded",
  "createdAt": "2024-01-15T14:30:00Z"
}
```

### 2. Process Purchase Payment

**POST** `/api/payment/purchase/{purchaseId}`

Process payment for factory purchasing marketplace items.

**Headers:**
```
Authorization: Bearer <factory-token>
```

**Request Body:**
```json
{
  "factoryId": 2
}
```

**Response:**
```json
{
  "isSuccess": true,
  "message": "Payment processed successfully",
  "paymentIntentId": "pi_1234567890",
  "clientSecret": "pi_1234567890_secret_abc123",
  "amount": 62.50,
  "currency": "usd",
  "status": "succeeded",
  "createdAt": "2024-01-15T16:00:00Z"
}
```

### 3. Create Payment Intent

**POST** `/api/payment/create-intent`

Create a payment intent.

**Headers:**
```
Authorization: Bearer <token>
```

**Request Body:**
```json
{
  "amount": 100.00,
  "currency": "usd",
  "description": "Payment for recycling materials",
  "customerEmail": "customer@example.com"
}
```

**Response:**
```json
{
  "isSuccess": true,
  "message": "Payment intent created successfully",
  "paymentIntentId": "pi_1234567890",
  "clientSecret": "pi_1234567890_secret_abc123",
  "amount": 100.00,
  "currency": "usd",
  "status": "requires_payment_method",
  "createdAt": "2024-01-15T10:00:00Z"
}
```

---

## Error Responses

### Standard Error Format

```json
{
  "error": "Error message description",
  "details": "Additional error details if available",
  "timestamp": "2024-01-15T10:30:00Z",
  "requestId": "req_1234567890"
}
```

### Common HTTP Status Codes

- **200 OK**: Request successful
- **201 Created**: Resource created successfully
- **400 Bad Request**: Invalid request data
- **401 Unauthorized**: Authentication required
- **403 Forbidden**: Insufficient permissions
- **404 Not Found**: Resource not found
- **422 Unprocessable Entity**: Validation errors
- **500 Internal Server Error**: Server error

### Validation Error Example

```json
{
  "error": "Validation failed",
  "details": {
    "email": ["Email is required"],
    "password": ["Password must be at least 8 characters"]
  },
  "timestamp": "2024-01-15T10:30:00Z",
  "requestId": "req_1234567890"
}
```

---

## Rate Limiting

The API implements rate limiting to ensure fair usage:

- **Authentication endpoints**: 10 requests per minute
- **General endpoints**: 100 requests per minute
- **Admin endpoints**: 50 requests per minute

Rate limit headers are included in responses:

```
X-RateLimit-Limit: 100
X-RateLimit-Remaining: 95
X-RateLimit-Reset: 1642234567
```

---

## Testing Scenarios

### Complete Workflow Test

#### 1. User Registration and Login
```bash
# Register user
curl -X POST https://api.nadafa.com/v1/api/auth/register/user \
  -H "Content-Type: application/json" \
  -d '{
    "name": "Test User",
    "email": "test@example.com",
    "password": "TestPassword123!",
    "phone": "+1234567890",
    "address": "123 Test St, Test City"
  }'

# Login
curl -X POST https://api.nadafa.com/v1/api/auth/login \
  -H "Content-Type: application/json" \
  -d '{
    "email": "test@example.com",
    "password": "TestPassword123!"
  }'
```

#### 2. Create Pickup Request
```bash
# Create pickup request
curl -X POST https://api.nadafa.com/v1/api/pickup-request \
  -H "Authorization: Bearer <user-token>" \
  -H "Content-Type: application/json" \
  -d '{
    "materialType": "Plastic",
    "quantity": 25.0,
    "unit": "Kg",
    "proposedPricePerUnit": 2.00,
    "description": "Test plastic materials"
  }'
```

#### 3. Admin Approval
```bash
# Approve request
curl -X PUT https://api.nadafa.com/v1/api/pickup-request/{request-id}/approve \
  -H "Authorization: Bearer <admin-token>" \
  -H "Content-Type: application/json" \
  -d '{
    "approvedPricePerUnit": 2.00,
    "notes": "Test approval"
  }'
```

#### 4. Process Payment
```bash
# Process payment
curl -X POST https://api.nadafa.com/v1/api/payment/pickup/{request-id} \
  -H "Authorization: Bearer <admin-token>" \
  -H "Content-Type: application/json" \
  -d '{
    "amount": 50.00
  }'
```

#### 5. Factory Purchase
```bash
# Purchase item
curl -X POST https://api.nadafa.com/v1/api/marketplace/{item-id}/purchase \
  -H "Authorization: Bearer <factory-token>" \
  -H "Content-Type: application/json" \
  -d '{
    "marketplaceItemId": "{item-id}",
    "quantity": 20.0,
    "pricePerUnit": 2.00,
    "stripePaymentIntentId": "pi_test123"
  }'
```

### Role-Based Access Test

#### Test User Permissions
```bash
# Should succeed
curl -X GET https://api.nadafa.com/v1/api/pickup-request/my-requests \
  -H "Authorization: Bearer <user-token>"

# Should fail (403 Forbidden)
curl -X GET https://api.nadafa.com/v1/api/admin/dashboard \
  -H "Authorization: Bearer <user-token>"
```

#### Test Admin Permissions
```bash
# Should succeed
curl -X GET https://api.nadafa.com/v1/api/admin/dashboard \
  -H "Authorization: Bearer <admin-token>"

# Should succeed
curl -X GET https://api.nadafa.com/v1/api/pickup-request/pending \
  -H "Authorization: Bearer <admin-token>"
```

#### Test Factory Permissions
```bash
# Should succeed
curl -X GET https://api.nadafa.com/v1/api/purchase/my-purchases \
  -H "Authorization: Bearer <factory-token>"

# Should fail (403 Forbidden)
curl -X GET https://api.nadafa.com/v1/api/pickup-request/pending \
  -H "Authorization: Bearer <factory-token>"
```

---

## Postman Collection

A complete Postman collection is available for testing all endpoints:

**Download**: [NADAFA_API_Collection.postman_collection.json](https://api.nadafa.com/docs/postman-collection)

### Collection Structure

1. **Authentication**
   - User Registration
   - Factory Registration
   - Login
   - Token Validation

2. **Pickup Requests**
   - Create Request
   - Get User Requests
   - Get Pending Requests
   - Approve/Reject Requests

3. **Marketplace**
   - Browse Items
   - Search Items
   - Purchase Items

4. **Purchases**
   - Get Factory Purchases
   - Confirm Payments

5. **Admin**
   - Dashboard Stats
   - Request Management
   - Payment History

6. **Notifications**
   - Get Notifications
   - Mark as Read
   - Unread Count

7. **Payments**
   - Process Payments
   - Create Payment Intents

### Environment Variables

Set up environment variables in Postman:

```
base_url: https://api.nadafa.com/v1
user_token: <user-jwt-token>
admin_token: <admin-jwt-token>
factory_token: <factory-jwt-token>
```

---

## SDKs and Libraries

### JavaScript/TypeScript

```bash
npm install @nadafa/api-client
```

```javascript
import { NadafaClient } from '@nadafa/api-client';

const client = new NadafaClient({
  baseUrl: 'https://api.nadafa.com/v1',
  token: 'your-jwt-token'
});

// Create pickup request
const request = await client.pickupRequests.create({
  materialType: 'Plastic',
  quantity: 25.0,
  unit: 'Kg',
  proposedPricePerUnit: 2.00,
  description: 'Test materials'
});
```

### Python

```bash
pip install nadafa-api-client
```

```python
from nadafa_api_client import NadafaClient

client = NadafaClient(
    base_url='https://api.nadafa.com/v1',
    token='your-jwt-token'
)

# Create pickup request
request = client.pickup_requests.create({
    'materialType': 'Plastic',
    'quantity': 25.0,
    'unit': 'Kg',
    'proposedPricePerUnit': 2.00,
    'description': 'Test materials'
})
```

---

## Support and Contact

For API support and questions:

- **Email**: api-support@nadafa.com
- **Documentation**: https://docs.nadafa.com
- **Status Page**: https://status.nadafa.com
- **GitHub**: https://github.com/nadafa/api

---

## Versioning

The API uses semantic versioning. The current version is v1.

- **v1**: Current stable version
- **v2**: Coming soon (backward compatible)

Version is specified in the URL: `https://api.nadafa.com/v1/`

---

## Changelog

### v1.0.0 (2024-01-15)
- Initial API release
- Complete CRUD operations for all entities
- JWT authentication
- Role-based access control
- Stripe payment integration
- Email notifications
- Comprehensive documentation

