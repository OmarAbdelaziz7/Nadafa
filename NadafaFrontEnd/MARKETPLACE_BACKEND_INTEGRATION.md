# 🏪 Marketplace Backend Integration Guide

## ⚠️ **IMPORTANT: Updated Requirements**

### **Recent Changes Made:**
- **✅ Removed seller ratings and purchase counts** - No rating system needed
- **✅ Removed pickup options** - All materials are shipped (no pickup/delivery options)  
- **✅ Removed material condition tags** - No fair/good/excellent classifications
- **✅ Added anonymous reviews system** - Comments only, no ratings
- **✅ Added single material detail page** - Full material view with reviews

## Overview

The Marketplace interface allows factories to browse and purchase recyclable materials from verified sellers. This document outlines the backend integration requirements for the .NET API.

## 🎯 System Architecture

### Frontend Components
- **MarketplaceComponent**: Main interface for browsing materials
- **MaterialDetailComponent**: Single material view with reviews
- **MarketplaceService**: Service layer for API communication
- **Material Models**: TypeScript interfaces for data structures

### Backend Requirements
- **.NET Web API**: RESTful API endpoints
- **Entity Framework**: Database ORM
- **Authentication**: JWT-based authentication
- **Authorization**: Role-based access control

## 🔄 **Updated Database Schema Changes**

### **Required Changes:**

1. **Remove from Sellers table:**
   - `Rating` column
   - `TotalSales` column

2. **Remove from Materials table:**
   - `Condition` column (no more fair/good/excellent)
   - `PickupOptions` related tables/columns

3. **Add new MaterialReviews table:**
```sql
CREATE TABLE MaterialReviews (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    MaterialId UNIQUEIDENTIFIER NOT NULL,
    Comment NVARCHAR(MAX) NOT NULL,
    HelpfulCount INT DEFAULT 0,
    CreatedAt DATETIME2 DEFAULT GETDATE(),
    FOREIGN KEY (MaterialId) REFERENCES Materials(Id)
);
```

4. **Update PurchaseRequests table:**
   - Remove `PickupOption` column
   - Update `DeliveryAddress` to be required (not optional)
   - Rename `PreferredPickupDate` to `PreferredDeliveryDate`

## 📊 Database Schema

### Required Tables

#### 1. Sellers Table
```sql
CREATE TABLE Sellers (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    UserId UNIQUEIDENTIFIER NOT NULL, -- Reference to AspNetUsers
    Name NVARCHAR(255) NOT NULL,
    Email NVARCHAR(255) NOT NULL,
    Phone NVARCHAR(50),
    AddressStreet NVARCHAR(255),
    AddressCity NVARCHAR(100),
    AddressState NVARCHAR(100),
    AddressZipCode NVARCHAR(20),
    AddressCountry NVARCHAR(100),
    Rating DECIMAL(3,2) DEFAULT 0.0, -- 0.00 to 5.00
    TotalSales INT DEFAULT 0,
    JoinedDate DATETIME2 DEFAULT GETDATE(),
    Verified BIT DEFAULT 0,
    ProfileImageUrl NVARCHAR(500),
    CreatedAt DATETIME2 DEFAULT GETDATE(),
    UpdatedAt DATETIME2 DEFAULT GETDATE()
);
```

#### 2. Materials Table
```sql
CREATE TABLE Materials (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    SellerId UNIQUEIDENTIFIER NOT NULL,
    Name NVARCHAR(255) NOT NULL,
    Category NVARCHAR(50) NOT NULL, -- plastic, metal, paper, glass, etc.
    Description NVARCHAR(2000),
    Quantity DECIMAL(10,2) NOT NULL,
    Unit NVARCHAR(20) NOT NULL, -- kg, tons, pieces, etc.
    PricePerUnit DECIMAL(10,2) NOT NULL,
    TotalPrice AS (Quantity * PricePerUnit) PERSISTED,
    Condition NVARCHAR(20) NOT NULL, -- excellent, good, fair, poor
    LocationStreet NVARCHAR(255),
    LocationCity NVARCHAR(100),
    LocationState NVARCHAR(100),
    LocationZipCode NVARCHAR(20),
    LocationCountry NVARCHAR(100),
    AvailableFrom DATETIME2 NOT NULL,
    AvailableUntil DATETIME2,
    Specifications NVARCHAR(MAX), -- JSON string
    Certifications NVARCHAR(MAX), -- JSON array
    PickupOptions NVARCHAR(MAX), -- JSON array
    Status NVARCHAR(20) DEFAULT 'available', -- available, pending, sold, removed
    CreatedAt DATETIME2 DEFAULT GETDATE(),
    UpdatedAt DATETIME2 DEFAULT GETDATE(),
    
    FOREIGN KEY (SellerId) REFERENCES Sellers(Id)
);
```

#### 3. Material Images Table
```sql
CREATE TABLE MaterialImages (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    MaterialId UNIQUEIDENTIFIER NOT NULL,
    ImageUrl NVARCHAR(500) NOT NULL,
    IsPrimary BIT DEFAULT 0,
    CreatedAt DATETIME2 DEFAULT GETDATE(),
    
    FOREIGN KEY (MaterialId) REFERENCES Materials(Id) ON DELETE CASCADE
);
```

#### 4. Purchase Requests Table
```sql
CREATE TABLE PurchaseRequests (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    MaterialId UNIQUEIDENTIFIER NOT NULL,
    FactoryId UNIQUEIDENTIFIER NOT NULL, -- Reference to AspNetUsers
    RequestedQuantity DECIMAL(10,2) NOT NULL,
    ProposedPricePerUnit DECIMAL(10,2),
    Message NVARCHAR(1000),
    DeliveryAddressStreet NVARCHAR(255),
    DeliveryAddressCity NVARCHAR(100),
    DeliveryAddressState NVARCHAR(100),
    DeliveryAddressZipCode NVARCHAR(20),
    DeliveryAddressCountry NVARCHAR(100),
    PreferredPickupDate DATETIME2,
    PickupOption NVARCHAR(50), -- seller_delivery, buyer_pickup, courier_service, shipping
    Status NVARCHAR(20) DEFAULT 'pending', -- pending, approved, rejected, completed
    CreatedAt DATETIME2 DEFAULT GETDATE(),
    UpdatedAt DATETIME2 DEFAULT GETDATE(),
    
    FOREIGN KEY (MaterialId) REFERENCES Materials(Id),
    FOREIGN KEY (FactoryId) REFERENCES AspNetUsers(Id)
);
```

## 🔌 Required API Endpoints

### 1. Materials Endpoints

#### GET `/api/materials`
**Purpose**: Get paginated list of available materials with filtering and sorting

**Query Parameters**:
```typescript
{
  // Filtering
  categories?: string[], // comma-separated: "plastic,metal"
  minPrice?: number,
  maxPrice?: number,
  minQuantity?: number,
  maxQuantity?: number,
  city?: string,
  state?: string,
  radius?: number, // search radius in km
  availableFrom?: string, // ISO date
  availableUntil?: string, // ISO date
  searchTerm?: string,
  
  // Pagination
  page?: number, // 1-based
  pageSize?: number, // default 12
  
  // Sorting
  sortBy?: string, // "price" | "quantity" | "rating" | "date" | "distance"
  sortDirection?: string // "asc" | "desc"
}
```

**Response**:
```json
{
  "data": [
    {
      "id": "guid",
      "sellerId": "guid",
      "name": "High-Grade PET Plastic Bottles",
      "category": "plastic",
      "description": "Clean, sorted PET plastic bottles...",
      "quantity": 500,
      "unit": "kg",
      "pricePerUnit": 0.85,
      "totalPrice": 425,
      "condition": "excellent",
      "images": ["url1", "url2"],
      "location": {
        "street": "123 Eco Street",
        "city": "Austin",
        "state": "Texas",
        "zipCode": "78701",
        "country": "USA"
      },
      "availableFrom": "2024-01-15T00:00:00Z",
      "availableUntil": "2024-02-15T00:00:00Z",
      "specifications": {
        "purity": "95%",
        "contamination": "None"
      },
      "certifications": ["ISO 14001", "FSC Certified"],
      "pickupOptions": ["buyer_pickup", "courier_service"],
      "status": "available",
      "createdAt": "2024-01-15T10:30:00Z",
      "updatedAt": "2024-01-15T10:30:00Z",
      "seller": {
        "id": "guid",
        "name": "GreenTech Recycling Co.",
        "email": "contact@greentech.com",
        "phone": "+1 555-0101",
        "address": {
          "street": "123 Eco Street",
          "city": "Austin",
          "state": "Texas",
          "zipCode": "78701",
          "country": "USA"
        },
        "rating": 4.8,
        "totalSales": 150,
        "joinedDate": "2022-01-15T00:00:00Z",
        "verified": true,
        "profileImage": "https://example.com/profile.jpg"
      }
    }
  ],
  "pagination": {
    "currentPage": 1,
    "pageSize": 12,
    "totalItems": 45,
    "totalPages": 4,
    "hasNext": true,
    "hasPrevious": false
  }
}
```

#### GET `/api/materials/{id}`
**Purpose**: Get detailed information about a specific material

**Response**: Same as single material object above

### 2. Material Details Endpoint (NEW)

#### GET `/api/materials/{id}`
**Purpose**: Get detailed information about a specific material
**Authentication**: Required (Factory role)

**Response**:
```json
{
  "id": "guid",
  "sellerId": "guid", 
  "name": "string",
  "category": "string",
  "description": "string",
  "quantity": 500,
  "unit": "kg",
  "pricePerUnit": 0.85,
  "totalPrice": 425.0,
  "images": ["url1", "url2"],
  "location": {
    "street": "string",
    "city": "string", 
    "state": "string",
    "zipCode": "string",
    "country": "string"
  },
  "availableFrom": "2024-01-15T00:00:00Z",
  "availableUntil": "2024-03-15T00:00:00Z",
  "specifications": {
    "purity": "95%",
    "grade": "Food-grade"
  },
  "certifications": ["ISO 14001", "FSC Certified"],
  "status": "available",
  "seller": {
    "id": "guid",
    "name": "GreenTech Recycling Co.",
    "email": "contact@greentech.com",
    "phone": "+1 555-0101",
    "verified": true,
    "joinedDate": "2022-01-15T00:00:00Z",
    "profileImage": "url"
  }
}
```

### 3. Material Reviews Endpoints (NEW)

#### GET `/api/materials/{id}/reviews`
**Purpose**: Get anonymous reviews for a specific material
**Authentication**: Required (Factory role)

**Response**:
```json
{
  "reviews": [
    {
      "id": "guid",
      "materialId": "guid",
      "comment": "Great quality materials, exactly as described.",
      "helpful": 5,
      "createdAt": "2024-01-10T00:00:00Z"
    }
  ],
  "totalCount": 3
}
```

### 4. Sellers Endpoints

#### GET `/api/sellers/{id}`
**Purpose**: Get detailed seller information

#### GET `/api/sellers/{id}/materials`
**Purpose**: Get all materials from a specific seller

### 3. Purchase Request Endpoints

#### POST `/api/purchase-requests`
**Purpose**: Submit a purchase request for materials

**Request Body**:
```json
{
  "materialId": "guid",
  "requestedQuantity": 100,
  "proposedPricePerUnit": 0.90,
  "message": "Interested in purchasing for our recycling facility",
  "deliveryAddress": {
    "street": "456 Factory Road",
    "city": "Dallas",
    "state": "Texas",
    "zipCode": "75201",
    "country": "USA"
  },
  "preferredPickupDate": "2024-02-01T09:00:00Z",
  "pickupOption": "courier_service"
}
```

**Response**:
```json
{
  "success": true,
  "message": "Purchase request submitted successfully",
  "requestId": "guid"
}
```

#### GET `/api/purchase-requests`
**Purpose**: Get factory's purchase request history

#### GET `/api/purchase-requests/{id}`
**Purpose**: Get specific purchase request details

## 🔒 Authentication & Authorization

### Required Claims
- **Factory Role**: `role:factory`
- **User ID**: `sub` or `nameid`
- **Email**: `email`

### Endpoint Security
```csharp
[Authorize(Roles = "Factory")]
[HttpGet("materials")]
public async Task<IActionResult> GetMaterials([FromQuery] MaterialsQuery query)
{
    // Implementation
}
```

## 📝 .NET Implementation Examples

### 1. Material Model
```csharp
public class Material
{
    public Guid Id { get; set; }
    public Guid SellerId { get; set; }
    public string Name { get; set; }
    public MaterialCategory Category { get; set; }
    public string Description { get; set; }
    public decimal Quantity { get; set; }
    public string Unit { get; set; }
    public decimal PricePerUnit { get; set; }
    public decimal TotalPrice => Quantity * PricePerUnit;
    public MaterialCondition Condition { get; set; }
    public Address Location { get; set; }
    public DateTime AvailableFrom { get; set; }
    public DateTime? AvailableUntil { get; set; }
    public Dictionary<string, object> Specifications { get; set; }
    public List<string> Certifications { get; set; }
    public List<PickupOption> PickupOptions { get; set; }
    public MaterialStatus Status { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    
    // Navigation properties
    public Seller Seller { get; set; }
    public List<MaterialImage> Images { get; set; }
}

public enum MaterialCategory
{
    Plastic, Metal, Paper, Glass, Electronics, Textiles, Rubber, Wood, Organic, Other
}

public enum MaterialCondition
{
    Excellent, Good, Fair, Poor
}

public enum MaterialStatus
{
    Available, Pending, Sold, Removed
}

public enum PickupOption
{
    SellerDelivery, BuyerPickup, CourierService, Shipping
}
```

### 2. Materials Controller
```csharp
[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Factory")]
public class MaterialsController : ControllerBase
{
    private readonly IMaterialService _materialService;
    
    public MaterialsController(IMaterialService materialService)
    {
        _materialService = materialService;
    }
    
    [HttpGet]
    public async Task<ActionResult<PagedResponse<MaterialListDto>>> GetMaterials(
        [FromQuery] MaterialsQuery query)
    {
        var result = await _materialService.GetMaterialsAsync(query);
        return Ok(result);
    }
    
    [HttpGet("{id}")]
    public async Task<ActionResult<MaterialDetailDto>> GetMaterial(Guid id)
    {
        var material = await _materialService.GetMaterialByIdAsync(id);
        if (material == null)
            return NotFound();
            
        return Ok(material);
    }
}
```

### 3. Material Service Interface
```csharp
public interface IMaterialService
{
    Task<PagedResponse<MaterialListDto>> GetMaterialsAsync(MaterialsQuery query);
    Task<MaterialDetailDto> GetMaterialByIdAsync(Guid id);
    Task<List<MaterialCategory>> GetCategoriesAsync();
    Task<PagedResponse<MaterialListDto>> SearchMaterialsAsync(string searchTerm, int page, int pageSize);
}
```

### 4. DTOs
```csharp
public class MaterialListDto
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public MaterialCategory Category { get; set; }
    public string Description { get; set; }
    public decimal Quantity { get; set; }
    public string Unit { get; set; }
    public decimal PricePerUnit { get; set; }
    public decimal TotalPrice { get; set; }
    public MaterialCondition Condition { get; set; }
    public List<string> Images { get; set; }
    public AddressDto Location { get; set; }
    public DateTime AvailableFrom { get; set; }
    public DateTime? AvailableUntil { get; set; }
    public List<PickupOption> PickupOptions { get; set; }
    public SellerSummaryDto Seller { get; set; }
}

public class SellerSummaryDto
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public decimal Rating { get; set; }
    public int TotalSales { get; set; }
    public bool Verified { get; set; }
    public string ProfileImage { get; set; }
}

public class MaterialsQuery
{
    public List<MaterialCategory>? Categories { get; set; }
    public decimal? MinPrice { get; set; }
    public decimal? MaxPrice { get; set; }
    public decimal? MinQuantity { get; set; }
    public decimal? MaxQuantity { get; set; }
    public List<MaterialCondition>? Conditions { get; set; }
    public List<PickupOption>? PickupOptions { get; set; }
    public string? City { get; set; }
    public string? State { get; set; }
    public decimal? SellerRating { get; set; }
    public string? SearchTerm { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 12;
    public string? SortBy { get; set; } = "date";
    public string? SortDirection { get; set; } = "desc";
}
```

## 🚀 Frontend Service Integration

### Update MarketplaceService

Replace the dummy data methods with HTTP calls:

```typescript
import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';

@Injectable({
  providedIn: 'root'
})
export class MarketplaceService {
  private http = inject(HttpClient);
  private baseUrl = `${environment.apiUrl}/api`;

  getMaterialListings(filters?: MarketplaceFilters, sort?: MarketplaceSortOptions): Observable<PagedResponse<MaterialListing>> {
    let params = new HttpParams();
    
    if (filters) {
      if (filters.categories?.length) {
        params = params.set('categories', filters.categories.join(','));
      }
      if (filters.minPrice !== undefined) {
        params = params.set('minPrice', filters.minPrice.toString());
      }
      if (filters.maxPrice !== undefined) {
        params = params.set('maxPrice', filters.maxPrice.toString());
      }
      if (filters.minQuantity !== undefined) {
        params = params.set('minQuantity', filters.minQuantity.toString());
      }
      if (filters.conditions?.length) {
        params = params.set('conditions', filters.conditions.join(','));
      }
      if (filters.location?.city) {
        params = params.set('city', filters.location.city);
      }
      if (filters.location?.state) {
        params = params.set('state', filters.location.state);
      }
      if (filters.sellerRating !== undefined) {
        params = params.set('sellerRating', filters.sellerRating.toString());
      }
      if (filters.searchTerm) {
        params = params.set('searchTerm', filters.searchTerm);
      }
    }
    
    if (sort) {
      params = params.set('sortBy', sort.field);
      params = params.set('sortDirection', sort.direction);
    }
    
    return this.http.get<PagedResponse<MaterialListing>>(`${this.baseUrl}/materials`, { params });
  }

  getMaterialById(id: string): Observable<MaterialListing> {
    return this.http.get<MaterialListing>(`${this.baseUrl}/materials/${id}`);
  }

  submitPurchaseRequest(request: PurchaseRequest): Observable<{ success: boolean; message: string; requestId?: string }> {
    return this.http.post<{ success: boolean; message: string; requestId?: string }>(
      `${this.baseUrl}/purchase-requests`, 
      request
    );
  }
}

interface PagedResponse<T> {
  data: T[];
  pagination: {
    currentPage: number;
    pageSize: number;
    totalItems: number;
    totalPages: number;
    hasNext: boolean;
    hasPrevious: boolean;
  };
}
```

### Environment Configuration

Add API configuration to environment files:

```typescript
// src/environments/environment.ts
export const environment = {
  production: false,
  apiUrl: 'https://localhost:7001', // Your .NET API URL
  // ... other config
};

// src/environments/environment.prod.ts
export const environment = {
  production: true,
  apiUrl: 'https://your-production-api.com',
  // ... other config
};
```

## 🔧 Setup Instructions

### 1. Database Setup
1. Run Entity Framework migrations
2. Seed initial data (categories, sample sellers/materials)
3. Configure indexes for performance

### 2. API Configuration
1. Configure CORS for Angular app
2. Set up JWT authentication
3. Configure file upload for images
4. Set up logging and error handling

### 3. Frontend Updates
1. Replace `MarketplaceService` dummy methods with HTTP calls
2. Update environment configuration
3. Add error handling and loading states
4. Test with real API endpoints

### 4. Testing
1. Unit tests for services and controllers
2. Integration tests for API endpoints
3. E2E tests for complete user flows

## 📊 Performance Considerations

### Database Optimization
- Add indexes on frequently queried columns
- Use pagination for large datasets
- Consider caching for categories and static data

### API Optimization
- Implement response caching
- Use async/await patterns
- Optimize database queries with proper includes

### Frontend Optimization
- Implement virtual scrolling for large lists
- Add image lazy loading
- Use OnPush change detection strategy

## 🔍 Monitoring & Logging

### Required Logging
- API request/response times
- Search queries and filters used
- Purchase request submissions
- Error tracking and alerting

### Analytics
- Popular material categories
- Search terms and conversion rates
- Seller performance metrics
- User engagement patterns

This integration guide provides a complete foundation for connecting the Angular marketplace interface with a .NET backend API.
