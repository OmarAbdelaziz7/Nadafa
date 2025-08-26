import { Injectable } from '@angular/core';
import { Observable, of, BehaviorSubject } from 'rxjs';
import { delay, map } from 'rxjs/operators';
import {
    MaterialListing,
    Seller,
    Material,
    MaterialCategory,
    MaterialStatus,
    MarketplaceFilters,
    MarketplaceSortOptions,
    PurchaseRequest,
    MaterialReview
} from '../../shared/models/marketplace.models';

@Injectable({
    providedIn: 'root'
})
export class MarketplaceService {
    private materialsSubject = new BehaviorSubject<MaterialListing[]>([]);
    public materials$ = this.materialsSubject.asObservable();

    private dummyData: MaterialListing[] = [];

    constructor() {
        this.generateDummyData();
        this.materialsSubject.next(this.dummyData);
    }

    private generateDummyData(): void {
        const sellers: Seller[] = [
            {
                id: '1',
                name: 'GreenTech Recycling Co.',
                email: 'contact@greentech.com',
                phone: '+1 555-0101',
                address: {
                    street: '123 Eco Street',
                    city: 'Austin',
                    state: 'Texas',
                    zipCode: '78701',
                    country: 'USA'
                },
                joinedDate: new Date('2022-01-15'),
                verified: true,
                profileImage: 'https://via.placeholder.com/100x100/4CAF50/white?text=GT'
            },
            {
                id: '2',
                name: 'EcoCollect Solutions',
                email: 'sales@ecocollect.com',
                phone: '+1 555-0102',
                address: {
                    street: '456 Sustainable Ave',
                    city: 'Portland',
                    state: 'Oregon',
                    zipCode: '97201',
                    country: 'USA'
                },

                joinedDate: new Date('2022-03-20'),
                verified: true,
                profileImage: 'https://via.placeholder.com/100x100/2196F3/white?text=EC'
            },
            {
                id: '3',
                name: 'RecyclePro Industries',
                email: 'info@recyclepro.com',
                phone: '+1 555-0103',
                address: {
                    street: '789 Green Boulevard',
                    city: 'San Francisco',
                    state: 'California',
                    zipCode: '94102',
                    country: 'USA'
                },

                joinedDate: new Date('2021-11-10'),
                verified: true,
                profileImage: 'https://via.placeholder.com/100x100/FF9800/white?text=RP'
            },
            {
                id: '4',
                name: 'Urban Waste Solutions',
                email: 'hello@urbanwaste.com',
                phone: '+1 555-0104',
                address: {
                    street: '321 Metropolitan Dr',
                    city: 'New York',
                    state: 'New York',
                    zipCode: '10001',
                    country: 'USA'
                },

                joinedDate: new Date('2023-01-05'),
                verified: false,
                profileImage: 'https://via.placeholder.com/100x100/9C27B0/white?text=UW'
            }
        ];

        const materials: Material[] = [
            {
                id: 'm1',
                sellerId: '1',
                name: 'High-Grade PET Plastic Bottles',
                category: MaterialCategory.PLASTIC,
                description: 'Clean, sorted PET plastic bottles suitable for recycling into new bottles or textiles. Collected from office buildings and restaurants.',
                quantity: 500,
                unit: 'kg',
                pricePerUnit: 0.85,
                totalPrice: 425,
                images: [
                    'https://via.placeholder.com/400x300/4CAF50/white?text=PET+Bottles',
                    'https://via.placeholder.com/400x300/8BC34A/white?text=Sorted+Clean'
                ],
                location: {
                    street: '123 Eco Street',
                    city: 'Austin',
                    state: 'Texas',
                    zipCode: '78701',
                    country: 'USA'
                },
                availableFrom: new Date(),
                availableUntil: new Date(Date.now() + 30 * 24 * 60 * 60 * 1000),
                specifications: {
                    purity: '95%',
                    contamination: 'None',
                    grade: 'Food-grade'
                },
                certifications: ['ISO 14001', 'FSC Certified'],

                status: MaterialStatus.AVAILABLE,
                createdAt: new Date('2024-01-15'),
                updatedAt: new Date('2024-01-15')
            },
            {
                id: 'm2',
                sellerId: '2',
                name: 'Aluminum Cans - Mixed Brands',
                category: MaterialCategory.METAL,
                description: 'Aluminum beverage cans in excellent condition. Sorted and cleaned. Perfect for aluminum recycling facilities.',
                quantity: 200,
                unit: 'kg',
                pricePerUnit: 1.20,
                totalPrice: 240,

                images: [
                    'https://via.placeholder.com/400x300/2196F3/white?text=Aluminum+Cans',
                    'https://via.placeholder.com/400x300/03A9F4/white?text=Clean+Sorted'
                ],
                location: {
                    street: '456 Sustainable Ave',
                    city: 'Portland',
                    state: 'Oregon',
                    zipCode: '97201',
                    country: 'USA'
                },
                availableFrom: new Date(),
                specifications: {
                    alloyType: '3004',
                    thickness: '0.1mm average'
                },

                status: MaterialStatus.AVAILABLE,
                createdAt: new Date('2024-01-20'),
                updatedAt: new Date('2024-01-20')
            },
            {
                id: 'm3',
                sellerId: '3',
                name: 'Office Paper - White A4',
                category: MaterialCategory.PAPER,
                description: 'High-quality white office paper, single-sided printing. Excellent for paper recycling into new office paper products.',
                quantity: 1000,
                unit: 'kg',
                pricePerUnit: 0.15,
                totalPrice: 150,

                images: [
                    'https://via.placeholder.com/400x300/FF9800/white?text=Office+Paper',
                    'https://via.placeholder.com/400x300/FFC107/white?text=White+A4'
                ],
                location: {
                    street: '789 Green Boulevard',
                    city: 'San Francisco',
                    state: 'California',
                    zipCode: '94102',
                    country: 'USA'
                },
                availableFrom: new Date(),
                availableUntil: new Date(Date.now() + 14 * 24 * 60 * 60 * 1000),
                specifications: {
                    grade: 'Grade A',
                    moisture: '<8%',
                    contamination: 'Minimal ink only'
                },

                status: MaterialStatus.AVAILABLE,
                createdAt: new Date('2024-01-22'),
                updatedAt: new Date('2024-01-22')
            },
            {
                id: 'm4',
                sellerId: '1',
                name: 'Mixed Glass Containers',
                category: MaterialCategory.GLASS,
                description: 'Assorted glass jars and bottles in various colors. Clean and sorted by color. Suitable for glass manufacturing.',
                quantity: 300,
                unit: 'kg',
                pricePerUnit: 0.08,
                totalPrice: 24,

                images: [
                    'https://via.placeholder.com/400x300/4CAF50/white?text=Glass+Containers'
                ],
                location: {
                    street: '123 Eco Street',
                    city: 'Austin',
                    state: 'Texas',
                    zipCode: '78701',
                    country: 'USA'
                },
                availableFrom: new Date(),
                specifications: {
                    colors: 'Clear, Brown, Green',
                    metalCaps: 'Removed'
                },

                status: MaterialStatus.AVAILABLE,
                createdAt: new Date('2024-01-18'),
                updatedAt: new Date('2024-01-18')
            },
            {
                id: 'm5',
                sellerId: '4',
                name: 'Electronic Components - Mixed',
                category: MaterialCategory.ELECTRONICS,
                description: 'Assorted electronic components from office equipment. Contains valuable metals and rare earth elements.',
                quantity: 50,
                unit: 'kg',
                pricePerUnit: 15.00,
                totalPrice: 750,

                images: [
                    'https://via.placeholder.com/400x300/9C27B0/white?text=Electronics',
                    'https://via.placeholder.com/400x300/7B1FA2/white?text=Components'
                ],
                location: {
                    street: '321 Metropolitan Dr',
                    city: 'New York',
                    state: 'New York',
                    zipCode: '10001',
                    country: 'USA'
                },
                availableFrom: new Date(),
                specifications: {
                    hazardousMaterials: 'Properly handled',
                    dataWiped: 'Yes'
                },
                certifications: ['WEEE Compliance', 'R2 Certified'],

                status: MaterialStatus.AVAILABLE,
                createdAt: new Date('2024-01-25'),
                updatedAt: new Date('2024-01-25')
            },
            {
                id: 'm6',
                sellerId: '2',
                name: 'Cardboard Boxes - Various Sizes',
                category: MaterialCategory.PAPER,
                description: 'Clean cardboard boxes from shipping and packaging. Flattened and sorted by size and quality.',
                quantity: 800,
                unit: 'kg',
                pricePerUnit: 0.12,
                totalPrice: 96,

                images: [
                    'https://via.placeholder.com/400x300/2196F3/white?text=Cardboard+Boxes'
                ],
                location: {
                    street: '456 Sustainable Ave',
                    city: 'Portland',
                    state: 'Oregon',
                    zipCode: '97201',
                    country: 'USA'
                },
                availableFrom: new Date(),
                specifications: {
                    grade: 'OCC (Old Corrugated Cardboard)',
                    contamination: 'None'
                },

                status: MaterialStatus.AVAILABLE,
                createdAt: new Date('2024-01-21'),
                updatedAt: new Date('2024-01-21')
            }
        ];

        // Combine materials with sellers
        this.dummyData = materials.map(material => {
            const seller = sellers.find(s => s.id === material.sellerId)!;
            return { ...material, seller } as MaterialListing;
        });
    }

    getMaterialById(id: string): Observable<MaterialListing | null> {
        return of(this.dummyData).pipe(
            map(materials => materials.find(m => m.id === id) || null),
            delay(300)
        );
    }

    getMaterialListings(filters?: MarketplaceFilters, sort?: MarketplaceSortOptions): Observable<MaterialListing[]> {
        return of(this.dummyData).pipe(
            delay(300), // Simulate API delay
            map(materials => {
                let filtered = [...materials];

                // Apply filters
                if (filters) {
                    if (filters.categories?.length) {
                        filtered = filtered.filter(m => filters.categories!.includes(m.category));
                    }
                    if (filters.minPrice !== undefined) {
                        filtered = filtered.filter(m => m.pricePerUnit >= filters.minPrice!);
                    }
                    if (filters.maxPrice !== undefined) {
                        filtered = filtered.filter(m => m.pricePerUnit <= filters.maxPrice!);
                    }
                    if (filters.minQuantity !== undefined) {
                        filtered = filtered.filter(m => m.quantity >= filters.minQuantity!);
                    }
                    if (filters.maxQuantity !== undefined) {
                        filtered = filtered.filter(m => m.quantity <= filters.maxQuantity!);
                    }

                    if (filters.location?.city) {
                        filtered = filtered.filter(m =>
                            m.location.city.toLowerCase().includes(filters.location!.city!.toLowerCase())
                        );
                    }
                    if (filters.location?.state) {
                        filtered = filtered.filter(m =>
                            m.location.state.toLowerCase().includes(filters.location!.state!.toLowerCase())
                        );
                    }

                    if (filters.searchTerm) {
                        const term = filters.searchTerm.toLowerCase();
                        filtered = filtered.filter(m =>
                            m.name.toLowerCase().includes(term) ||
                            m.description.toLowerCase().includes(term) ||
                            m.seller.name.toLowerCase().includes(term)
                        );
                    }
                }

                // Apply sorting
                if (sort) {
                    filtered.sort((a, b) => {
                        let aValue: any, bValue: any;

                        switch (sort.field) {
                            case 'price':
                                aValue = a.pricePerUnit;
                                bValue = b.pricePerUnit;
                                break;
                            case 'quantity':
                                aValue = a.quantity;
                                bValue = b.quantity;
                                break;

                            case 'date':
                                aValue = a.createdAt.getTime();
                                bValue = b.createdAt.getTime();
                                break;
                            default:
                                return 0;
                        }

                        if (sort.direction === 'asc') {
                            return aValue > bValue ? 1 : -1;
                        } else {
                            return aValue < bValue ? 1 : -1;
                        }
                    });
                }

                return filtered;
            })
        );
    }



    getSellerById(id: string): Observable<Seller | undefined> {
        const seller = this.dummyData.find(m => m.sellerId === id)?.seller;
        return of(seller).pipe(delay(200));
    }

    getSellerMaterials(sellerId: string): Observable<MaterialListing[]> {
        const materials = this.dummyData.filter(m => m.sellerId === sellerId);
        return of(materials).pipe(delay(200));
    }

    submitPurchaseRequest(request: PurchaseRequest): Observable<{ success: boolean; message: string; requestId?: string }> {
        // Simulate API call
        return of({
            success: true,
            message: 'Purchase request submitted successfully. The seller will be notified and will respond within 24 hours.',
            requestId: 'REQ-' + Date.now()
        }).pipe(delay(1000));
    }

    getCategories(): MaterialCategory[] {
        return Object.values(MaterialCategory);
    }

    // Add method to get reviews for a material
    getMaterialReviews(materialId: string): Observable<MaterialReview[]> {
        // Mock reviews data
        const reviews: MaterialReview[] = [
            {
                id: '1',
                materialId: materialId,
                comment: 'Great quality materials, exactly as described. Fast shipping and well packaged.',
                createdAt: new Date('2024-01-10'),
                helpful: 5
            },
            {
                id: '2',
                materialId: materialId,
                comment: 'Good value for money. Materials were clean and sorted properly.',
                createdAt: new Date('2024-01-08'),
                helpful: 3
            },
            {
                id: '3',
                materialId: materialId,
                comment: 'Perfect for our manufacturing needs. Will definitely order again.',
                createdAt: new Date('2024-01-05'),
                helpful: 7
            }
        ];
        return of(reviews).pipe(delay(300));
    }
}
