export interface Seller {
    id: string;
    name: string;
    email: string;
    phone: string;
    address: {
        street: string;
        city: string;
        state: string;
        zipCode: string;
        country: string;
    };
    joinedDate: Date;
    verified: boolean;
    profileImage?: string;
}

export interface Material {
    id: string;
    sellerId: string;
    name: string;
    category: MaterialCategory;
    description: string;
    quantity: number;
    unit: string; // kg, tons, pieces, etc.
    pricePerUnit: number;
    totalPrice: number;
    images: string[];
    location: {
        street: string;
        city: string;
        state: string;
        zipCode: string;
        country: string;
    };
    availableFrom: Date;
    availableUntil?: Date;
    specifications?: Record<string, any>; // Additional specs like purity, grade, etc.
    certifications?: string[]; // Environmental certifications, quality standards
    status: MaterialStatus;
    createdAt: Date;
    updatedAt: Date;
}

export interface MaterialListing extends Material {
    seller: Seller;
}

export enum MaterialCategory {
    PLASTIC = 'plastic',
    METAL = 'metal',
    PAPER = 'paper',
    GLASS = 'glass',
    ELECTRONICS = 'electronics',
    TEXTILES = 'textiles',
    RUBBER = 'rubber',
    WOOD = 'wood',
    ORGANIC = 'organic',
    OTHER = 'other'
}

export enum MaterialStatus {
    AVAILABLE = 'available',
    PENDING = 'pending',
    SOLD = 'sold',
    REMOVED = 'removed'
}

export interface MarketplaceFilters {
    categories?: MaterialCategory[];
    minPrice?: number;
    maxPrice?: number;
    minQuantity?: number;
    maxQuantity?: number;
    location?: {
        city?: string;
        state?: string;
        radius?: number; // in km
    };
    availableFrom?: Date;
    availableUntil?: Date;
    searchTerm?: string;
}

export interface MarketplaceSortOptions {
    field: 'price' | 'quantity' | 'date' | 'distance';
    direction: 'asc' | 'desc';
}

export interface PurchaseRequest {
    materialId: string;
    factoryId: string;
    requestedQuantity: number;
    proposedPricePerUnit?: number;
    message?: string;
    deliveryAddress: {
        street: string;
        city: string;
        state: string;
        zipCode: string;
        country: string;
    };
    preferredDeliveryDate?: Date;
}

// Add review interface for the single item page
export interface MaterialReview {
    id: string;
    materialId: string;
    comment: string;
    createdAt: Date;
    helpful: number; // number of people who found it helpful
}
