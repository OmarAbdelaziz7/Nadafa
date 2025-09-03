# NADAFA Core Entities Implementation Guide

## Overview
This document outlines the implementation of core domain entities for the NADAFA recycling platform, including pickup requests, marketplace items, purchases, and notifications.

## Current Implementation Status

### ✅ Completed
- **Enums**: All required enums have been created
- **Entity Classes**: All core entities have been implemented
- **Clean Architecture**: Entities follow clean architecture principles
- **Build Success**: Project compiles successfully

### 🔄 In Progress
- **Database Migration**: Migration needs to be completed
- **Entity Integration**: Entities need to be fully integrated with DbContext

### 📋 Pending
- **Database Schema Update**: Tables need to be created in the database
- **Testing**: Sample data insertion and relationship verification
- **API Integration**: Controllers and services for the new entities

## Entity Architecture

### 1. Core Enums (`Domain/Entities/Enums.cs`)

```csharp
public enum MaterialType
{
    Paper = 1,
    Plastic = 2,
    Metal = 3,
    Glass = 4,
    Electronic = 5
}

public enum Unit
{
    Kg = 1,
    Tons = 2,
    Pieces = 3
}

public enum PickupStatus
{
    Pending = 1,
    Approved = 2,
    Rejected = 3,
    PickedUp = 4,
    Published = 5
}

public enum PaymentStatus
{
    Pending = 1,
    Completed = 2,
    Failed = 3,
    Refunded = 4
}

public enum NotificationType
{
    PickupApproved = 1,
    PaymentReceived = 2,
    ItemSold = 3,
    PurchaseConfirmed = 4
}
```

### 2. Core Entities

#### PickupRequest Entity
- **Purpose**: Represents user requests for pickup of recyclable materials
- **Key Properties**:
  - `Id`: Guid identifier
  - `UserId`: Reference to the requesting user
  - `MaterialType`: Type of recyclable material
  - `Quantity` & `Unit`: Amount and measurement unit
  - `ProposedPricePerUnit`: User's suggested price
  - `TotalEstimatedPrice`: Calculated total (Quantity × PricePerUnit)
  - `Status`: Current status of the request
  - `AdminId` & `AdminNotes`: Admin approval information
  - `ImageUrls`: List of images showing the materials

#### MarketplaceItem Entity
- **Purpose**: Represents approved items available for purchase by factories
- **Key Properties**:
  - `Id`: Guid identifier
  - `PickupRequestId`: Reference to the original pickup request
  - `UserId`: Original owner of the materials
  - `IsAvailable`: Whether the item can be purchased
  - `PublishedAt`: When the item was made available

#### Purchase Entity
- **Purpose**: Records factory purchases of marketplace items
- **Key Properties**:
  - `Id`: Guid identifier
  - `MarketplaceItemId`: Reference to the purchased item
  - `FactoryId`: Reference to the purchasing factory
  - `StripePaymentIntentId`: Payment processing reference
  - `PaymentStatus`: Current payment status

#### Notification Entity
- **Purpose**: System notifications for users
- **Key Properties**:
  - `Id`: Guid identifier
  - `UserId`: Recipient of the notification
  - `NotificationType`: Type of notification
  - `IsRead`: Whether the notification has been read

## Database Schema

### Current Tables (Existing)
- `Users`: User accounts with roles (User, Admin, Factory)
- `Factories`: Factory-specific information
- `Payments`: Payment records

### New Tables (To Be Added)
- `PickupRequests`: Material pickup requests
- `MarketplaceItems`: Available items for purchase
- `Purchases`: Factory purchase records
- `Notifications`: User notification system

## Entity Relationships

```mermaid
erDiagram
    Users ||--o{ PickupRequests : "creates"
    Users ||--o{ MarketplaceItems : "owns"
    Users ||--o{ Notifications : "receives"
    Users ||--o{ PickupRequests : "approves"
    
    Factories ||--o{ Purchases : "makes"
    Factories ||--o{ Payments : "processes"
    
    PickupRequests ||--o| MarketplaceItems : "becomes"
    MarketplaceItems ||--o| Purchases : "purchased as"
    
    PickupRequests {
        Guid Id PK
        int UserId FK
        int? AdminId FK
        MaterialType MaterialType
        decimal Quantity
        Unit Unit
        decimal ProposedPricePerUnit
        decimal TotalEstimatedPrice
        string Description
        List<string> ImageUrls
        PickupStatus Status
        DateTime RequestDate
        DateTime? ApprovedDate
        DateTime? PickupDate
        string AdminNotes
        DateTime CreatedAt
        DateTime UpdatedAt
    }
    
    MarketplaceItems {
        Guid Id PK
        Guid PickupRequestId FK
        int UserId FK
        MaterialType MaterialType
        decimal Quantity
        Unit Unit
        decimal PricePerUnit
        decimal TotalPrice
        string Description
        List<string> ImageUrls
        bool IsAvailable
        DateTime PublishedAt
        DateTime CreatedAt
        DateTime UpdatedAt
    }
    
    Purchases {
        Guid Id PK
        Guid MarketplaceItemId FK
        int FactoryId FK
        decimal Quantity
        decimal PricePerUnit
        decimal TotalAmount
        string StripePaymentIntentId
        PaymentStatus PaymentStatus
        DateTime PurchaseDate
        DateTime CreatedAt
        DateTime UpdatedAt
    }
    
    Notifications {
        Guid Id PK
        int UserId FK
        string Title
        string Message
        bool IsRead
        NotificationType NotificationType
        DateTime CreatedAt
    }
```

## Implementation Steps

### Step 1: Complete Database Migration
1. Uncomment the new entity DbSets in `NadafaDbContext`
2. Uncomment the entity configurations
3. Create and run the migration
4. Verify database schema

### Step 2: Enable Navigation Properties
1. Uncomment navigation properties in User and Factory entities
2. Ensure proper relationship configurations
3. Test entity loading with navigation properties

### Step 3: Create Services and Controllers
1. Implement `IPickupRequestService` and `PickupRequestService`
2. Implement `IMarketplaceService` and `MarketplaceService`
3. Implement `INotificationService` and `NotificationService`
4. Create corresponding controllers

### Step 4: Testing and Validation
1. Insert sample data
2. Test entity relationships
3. Verify CRUD operations
4. Test business logic flows

## Sample Data Insertion Commands

### Insert Sample Users
```sql
INSERT INTO Users (Name, Email, Address, Age, PasswordHash, Role, CreatedAt, IsActive)
VALUES 
('John Doe', 'john@example.com', '123 Main St', 30, 'hashed_password', 1, NOW(), 1),
('Admin User', 'admin@nadafa.com', 'Admin Address', 35, 'hashed_password', 2, NOW(), 1),
('Factory Corp', 'factory@example.com', 'Factory Address', 0, 'hashed_password', 3, NOW(), 1);
```

### Insert Sample Pickup Request
```sql
INSERT INTO PickupRequests (Id, UserId, MaterialType, Quantity, Unit, ProposedPricePerUnit, Description, Status, RequestDate, CreatedAt, UpdatedAt)
VALUES 
(UUID(), 1, 1, 50.0, 1, 0.50, 'Paper waste from office', 1, NOW(), NOW(), NOW());
```

## Testing Queries

### Verify User Relationships
```sql
SELECT u.Name, COUNT(pr.Id) as PickupRequestCount
FROM Users u
LEFT JOIN PickupRequests pr ON u.Id = pr.UserId
WHERE u.Role = 1
GROUP BY u.Id, u.Name;
```

### Verify Pickup Request to Marketplace Flow
```sql
SELECT 
    pr.Description,
    pr.Status,
    mi.IsAvailable,
    mi.PricePerUnit
FROM PickupRequests pr
LEFT JOIN MarketplaceItems mi ON pr.Id = mi.PickupRequestId
WHERE pr.Status IN (2, 5);
```

### Verify Factory Purchases
```sql
SELECT 
    f.Name as FactoryName,
    mi.Description as ItemDescription,
    p.Quantity,
    p.TotalAmount,
    p.PaymentStatus
FROM Purchases p
JOIN MarketplaceItems mi ON p.MarketplaceItemId = mi.Id
JOIN Factories f ON p.FactoryId = f.Id;
```

## Business Logic Flows

### 1. Pickup Request Flow
1. User creates pickup request
2. Admin reviews and approves/rejects
3. If approved, item is picked up
4. Item is published to marketplace

### 2. Marketplace Purchase Flow
1. Factory browses available items
2. Factory selects item and quantity
3. Payment is processed via Stripe
4. Purchase record is created
5. Notification is sent to seller

### 3. Notification System
- Pickup approved/rejected notifications
- Payment received notifications
- Item sold notifications
- Purchase confirmation notifications

## Security Considerations

### Data Validation
- All decimal values have proper ranges
- String lengths are properly constrained
- Required fields are enforced at both entity and database levels

### Access Control
- Users can only see their own pickup requests
- Admins can see and manage all requests
- Factories can only see available marketplace items

### Payment Security
- Stripe handles all payment processing
- Payment intent IDs are stored for tracking
- No sensitive payment data is stored locally

## Performance Considerations

### Database Indexes
- Primary keys on all entities
- Foreign key indexes for relationships
- Composite indexes for common query patterns

### Query Optimization
- Use navigation properties for related data
- Implement pagination for large result sets
- Consider caching for frequently accessed data

## Future Enhancements

### Planned Features
1. **Bulk Operations**: Support for bulk pickup requests
2. **Advanced Search**: Filtering and sorting for marketplace items
3. **Real-time Notifications**: WebSocket support for instant updates
4. **Analytics Dashboard**: Business intelligence and reporting
5. **Mobile App Support**: API endpoints optimized for mobile clients

### Scalability Considerations
1. **Database Partitioning**: For high-volume data
2. **Caching Strategy**: Redis for frequently accessed data
3. **Message Queues**: For async notification processing
4. **Microservices**: Potential split into domain-specific services

## Troubleshooting

### Common Issues
1. **Migration Failures**: Ensure no existing data conflicts
2. **Navigation Property Loading**: Check lazy loading configuration
3. **Image URL Storage**: Verify JSON conversion settings
4. **Foreign Key Constraints**: Ensure proper cascade delete settings

### Debug Commands
```bash
# Check migration status
dotnet ef migrations list --project Infrastructure --startup-project Presentation

# Generate SQL script
dotnet ef migrations script --project Infrastructure --startup-project Presentation

# Update database
dotnet ef database update --project Infrastructure --startup-project Presentation
```

## Conclusion

The NADAFA core entities implementation provides a solid foundation for the recycling platform. The clean architecture approach ensures maintainability and testability, while the comprehensive entity design supports all required business processes.

Next steps involve completing the database migration, implementing the service layer, and creating the API controllers to expose the functionality to clients.

---

**Last Updated**: September 1, 2025  
**Version**: 1.0  
**Status**: Implementation Complete, Migration Pending
