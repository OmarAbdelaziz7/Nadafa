import { Component, OnInit, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, Router } from '@angular/router';
import { MatCardModule } from '@angular/material/card';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatDividerModule } from '@angular/material/divider';
import { MatChipsModule } from '@angular/material/chips';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatBadgeModule } from '@angular/material/badge';
import { MatTabsModule } from '@angular/material/tabs';

import { MarketplaceService } from '../../../core/services/marketplace.service';
import { MaterialListing, MaterialReview } from '../../../shared/models/marketplace.models';

@Component({
    selector: 'app-material-detail',
    standalone: true,
    imports: [
        CommonModule,
        MatCardModule,
        MatButtonModule,
        MatIconModule,
        MatDividerModule,
        MatChipsModule,
        MatProgressSpinnerModule,
        MatBadgeModule,
        MatTabsModule
    ],
    template: `
    <div class="material-detail" *ngIf="!loading(); else loadingTemplate">
      <div class="detail-container" *ngIf="material(); else notFoundTemplate">
        <!-- Back Button -->
        <div class="back-section">
          <button mat-button (click)="goBack()" class="back-button">
            <mat-icon>arrow_back</mat-icon>
            Back to Marketplace
          </button>
        </div>

        <!-- Material Header -->
        <div class="material-header">
          <div class="material-images">
            <div class="main-image">
              <img [src]="selectedImage()" [alt]="material()!.name" />
            </div>
            <div class="thumbnail-images" *ngIf="material()!.images.length > 1">
              <img 
                *ngFor="let image of material()!.images" 
                [src]="image" 
                [alt]="material()!.name"
                [class.selected]="image === selectedImage()"
                (click)="selectImage(image)" />
            </div>
          </div>

          <div class="material-info">
            <h1>{{ material()!.name }}</h1>
            
            <div class="category-badge">
              <mat-chip>{{ formatCategoryName(material()!.category) }}</mat-chip>
            </div>

            <div class="price-section">
              <span class="price-per-unit">\${{ material()!.pricePerUnit }}/{{ material()!.unit }}</span>
              <span class="total-price">\${{ material()!.totalPrice }} total</span>
            </div>

            <div class="quantity-section">
              <mat-icon>inventory</mat-icon>
              <span>{{ material()!.quantity }} {{ material()!.unit }} available</span>
            </div>

            <div class="location-section">
              <mat-icon>location_on</mat-icon>
              <span>{{ material()!.location.city }}, {{ material()!.location.state }}</span>
            </div>

            <div class="availability-section">
              <mat-icon>schedule</mat-icon>
              <span>Available from {{ material()!.availableFrom | date:'mediumDate' }}</span>
              <span *ngIf="material()!.availableUntil"> until {{ material()!.availableUntil | date:'mediumDate' }}</span>
            </div>

            <div class="action-section">
              <button mat-raised-button color="primary" class="purchase-button" (click)="requestPurchase()">
                <mat-icon>shopping_cart</mat-icon>
                Request Purchase
              </button>
              <button mat-button class="contact-button" (click)="contactSeller()">
                <mat-icon>email</mat-icon>
                Contact Seller
              </button>
            </div>
          </div>
        </div>

        <!-- Seller Information -->
        <mat-card class="seller-card">
          <mat-card-header>
            <div mat-card-avatar class="seller-avatar">
              <img [src]="material()!.seller.profileImage" [alt]="material()!.seller.name">
              <mat-icon *ngIf="material()!.seller.verified" class="verified-badge">verified</mat-icon>
            </div>
            <mat-card-title>{{ material()!.seller.name }}</mat-card-title>
            <mat-card-subtitle>
              Member since {{ material()!.seller.joinedDate | date:'MMM yyyy' }}
              <span *ngIf="material()!.seller.verified" class="verified-text">• Verified Seller</span>
            </mat-card-subtitle>
          </mat-card-header>
        </mat-card>

        <!-- Tabbed Content -->
        <mat-tab-group class="detail-tabs">
          <!-- Description Tab -->
          <mat-tab label="Description">
            <div class="tab-content">
              <h3>Product Description</h3>
              <p>{{ material()!.description }}</p>

              <div *ngIf="material()!.specifications" class="specifications">
                <h4>Specifications</h4>
                <div class="spec-grid">
                  <div *ngFor="let spec of getSpecifications()" class="spec-item">
                    <span class="spec-label">{{ spec.key }}:</span>
                    <span class="spec-value">{{ spec.value }}</span>
                  </div>
                </div>
              </div>

              <div *ngIf="material()!.certifications?.length" class="certifications">
                <h4>Certifications</h4>
                <mat-chip-set>
                  <mat-chip *ngFor="let cert of material()!.certifications">{{ cert }}</mat-chip>
                </mat-chip-set>
              </div>
            </div>
          </mat-tab>

          <!-- Reviews Tab -->
          <mat-tab label="Reviews" [matBadge]="reviews().length" matBadgePosition="after">
            <div class="tab-content">
              <div class="reviews-header">
                <h3>Anonymous Reviews</h3>
                <p class="reviews-subtitle">What other buyers are saying about this material</p>
              </div>

              <div *ngIf="reviewsLoading()" class="reviews-loading">
                <mat-progress-spinner diameter="40" mode="indeterminate"></mat-progress-spinner>
                <p>Loading reviews...</p>
              </div>

              <div *ngIf="!reviewsLoading() && reviews().length === 0" class="no-reviews">
                <mat-icon>rate_review</mat-icon>
                <h4>No reviews yet</h4>
                <p>Be the first to purchase and review this material!</p>
              </div>

              <div *ngIf="!reviewsLoading() && reviews().length > 0" class="reviews-list">
                <div *ngFor="let review of reviews()" class="review-item">
                  <div class="review-header">
                    <div class="review-avatar">
                      <mat-icon>person</mat-icon>
                    </div>
                    <div class="review-meta">
                      <span class="review-author">Anonymous Buyer</span>
                      <span class="review-date">{{ review.createdAt | date:'mediumDate' }}</span>
                    </div>
                    <div class="review-helpful" *ngIf="review.helpful > 0">
                      <mat-icon>thumb_up</mat-icon>
                      <span>{{ review.helpful }}</span>
                    </div>
                  </div>
                  <div class="review-content">
                    <p>{{ review.comment }}</p>
                  </div>
                </div>
              </div>
            </div>
          </mat-tab>

          <!-- Shipping Tab -->
          <mat-tab label="Shipping & Delivery">
            <div class="tab-content">
              <h3>Shipping Information</h3>
              <div class="shipping-info">
                <div class="shipping-item">
                  <mat-icon>local_shipping</mat-icon>
                  <div>
                    <h4>Free Shipping</h4>
                    <p>All materials include free shipping to your location</p>
                  </div>
                </div>
                <div class="shipping-item">
                  <mat-icon>schedule</mat-icon>
                  <div>
                    <h4>Delivery Time</h4>
                    <p>Standard delivery within 5-7 business days</p>
                  </div>
                </div>
                <div class="shipping-item">
                  <mat-icon>verified_user</mat-icon>
                  <div>
                    <h4>Secure Packaging</h4>
                    <p>Materials are professionally packaged to ensure quality</p>
                  </div>
                </div>
                <div class="shipping-item">
                  <mat-icon>track_changes</mat-icon>
                  <div>
                    <h4>Tracking Available</h4>
                    <p>Track your shipment from pickup to delivery</p>
                  </div>
                </div>
              </div>
            </div>
          </mat-tab>
        </mat-tab-group>
      </div>
    </div>

    <!-- Loading Template -->
    <ng-template #loadingTemplate>
      <div class="loading-state">
        <mat-progress-spinner mode="indeterminate" diameter="50"></mat-progress-spinner>
        <p>Loading material details...</p>
      </div>
    </ng-template>

    <!-- Not Found Template -->
    <ng-template #notFoundTemplate>
      <div class="not-found-state">
        <mat-icon>error_outline</mat-icon>
        <h3>Material Not Found</h3>
        <p>The material you're looking for doesn't exist or has been removed.</p>
        <button mat-raised-button color="primary" (click)="goBack()">
          Back to Marketplace
        </button>
      </div>
    </ng-template>
  `,
    styles: [`
    .material-detail {
      min-height: calc(100vh - 80px);
      background: #f5f5f5;
      padding: 2rem 1rem;
    }

    .detail-container {
      max-width: 1200px;
      margin: 0 auto;
    }

    .back-section {
      margin-bottom: 2rem;
    }

    .back-button {
      color: var(--mat-sys-primary);
    }

    .material-header {
      display: grid;
      grid-template-columns: 1fr 1fr;
      gap: 3rem;
      margin-bottom: 2rem;
      background: white;
      padding: 2rem;
      border-radius: 12px;
      box-shadow: 0 2px 8px rgba(0,0,0,0.1);
    }

    .material-images {
      display: flex;
      flex-direction: column;
      gap: 1rem;
    }

    .main-image {
      width: 100%;
      height: 400px;
      border-radius: 8px;
      overflow: hidden;
      background: #f0f0f0;
    }

    .main-image img {
      width: 100%;
      height: 100%;
      object-fit: cover;
    }

    .thumbnail-images {
      display: flex;
      gap: 0.5rem;
      overflow-x: auto;
    }

    .thumbnail-images img {
      width: 80px;
      height: 80px;
      object-fit: cover;
      border-radius: 4px;
      cursor: pointer;
      border: 2px solid transparent;
      transition: border-color 0.2s ease;
    }

    .thumbnail-images img:hover {
      border-color: var(--mat-sys-primary);
    }

    .thumbnail-images img.selected {
      border-color: var(--mat-sys-primary);
    }

    .material-info {
      display: flex;
      flex-direction: column;
      gap: 1.5rem;
    }

    .material-info h1 {
      font-size: 2rem;
      font-weight: 700;
      margin: 0;
      color: var(--mat-sys-on-surface);
    }

    .category-badge mat-chip {
      background-color: var(--mat-sys-primary-container);
      color: var(--mat-sys-on-primary-container);
    }

    .price-section {
      display: flex;
      flex-direction: column;
      gap: 0.5rem;
    }

    .price-per-unit {
      font-size: 1.5rem;
      font-weight: 600;
      color: var(--mat-sys-primary);
    }

    .total-price {
      font-size: 1.25rem;
      color: var(--mat-sys-on-surface-variant);
    }

    .quantity-section,
    .location-section,
    .availability-section {
      display: flex;
      align-items: center;
      gap: 0.5rem;
      color: var(--mat-sys-on-surface-variant);
    }

    .action-section {
      display: flex;
      gap: 1rem;
      margin-top: 1rem;
    }

    .purchase-button {
      flex: 1;
      padding: 1rem 2rem;
      font-size: 1.1rem;
    }

    .seller-card {
      margin-bottom: 2rem;
    }

    .seller-avatar {
      position: relative;
    }

    .seller-avatar img {
      width: 40px;
      height: 40px;
      border-radius: 50%;
      object-fit: cover;
    }

    .verified-badge {
      position: absolute;
      bottom: -2px;
      right: -2px;
      font-size: 1rem;
      width: 1rem;
      height: 1rem;
      background: white;
      border-radius: 50%;
      color: var(--mat-sys-primary);
    }

    .verified-text {
      color: var(--mat-sys-primary);
      font-weight: 500;
    }

    .detail-tabs {
      background: white;
      border-radius: 12px;
      box-shadow: 0 2px 8px rgba(0,0,0,0.1);
    }

    .tab-content {
      padding: 2rem;
    }

    .tab-content h3 {
      margin: 0 0 1rem 0;
      color: var(--mat-sys-on-surface);
    }

    .tab-content h4 {
      margin: 1.5rem 0 1rem 0;
      color: var(--mat-sys-on-surface);
      font-size: 1.1rem;
    }

    .tab-content p {
      line-height: 1.6;
      color: var(--mat-sys-on-surface-variant);
    }

    .spec-grid {
      display: grid;
      grid-template-columns: repeat(auto-fit, minmax(250px, 1fr));
      gap: 1rem;
    }

    .spec-item {
      display: flex;
      justify-content: space-between;
      padding: 0.75rem;
      background: var(--mat-sys-surface-variant);
      border-radius: 4px;
    }

    .spec-label {
      font-weight: 500;
      color: var(--mat-sys-on-surface);
    }

    .spec-value {
      color: var(--mat-sys-on-surface-variant);
    }

    .certifications {
      margin-top: 1.5rem;
    }

    .reviews-header {
      margin-bottom: 2rem;
    }

    .reviews-subtitle {
      color: var(--mat-sys-on-surface-variant);
      margin: 0.5rem 0 0 0;
    }

    .reviews-loading,
    .no-reviews {
      text-align: center;
      padding: 2rem;
      color: var(--mat-sys-on-surface-variant);
    }

    .reviews-loading mat-progress-spinner {
      margin: 0 auto 1rem auto;
    }

    .no-reviews mat-icon {
      font-size: 3rem;
      width: 3rem;
      height: 3rem;
      opacity: 0.5;
      margin-bottom: 1rem;
    }

    .reviews-list {
      display: flex;
      flex-direction: column;
      gap: 1.5rem;
    }

    .review-item {
      padding: 1.5rem;
      background: var(--mat-sys-surface-variant);
      border-radius: 8px;
    }

    .review-header {
      display: flex;
      align-items: center;
      gap: 1rem;
      margin-bottom: 1rem;
    }

    .review-avatar {
      width: 40px;
      height: 40px;
      border-radius: 50%;
      background: var(--mat-sys-primary-container);
      display: flex;
      align-items: center;
      justify-content: center;
      color: var(--mat-sys-on-primary-container);
    }

    .review-meta {
      flex: 1;
      display: flex;
      flex-direction: column;
      gap: 0.25rem;
    }

    .review-author {
      font-weight: 500;
      color: var(--mat-sys-on-surface);
    }

    .review-date {
      font-size: 0.875rem;
      color: var(--mat-sys-on-surface-variant);
    }

    .review-helpful {
      display: flex;
      align-items: center;
      gap: 0.25rem;
      color: var(--mat-sys-primary);
      font-size: 0.875rem;
    }

    .review-helpful mat-icon {
      font-size: 1rem;
      width: 1rem;
      height: 1rem;
    }

    .review-content p {
      margin: 0;
      line-height: 1.6;
    }

    .shipping-info {
      display: flex;
      flex-direction: column;
      gap: 1.5rem;
    }

    .shipping-item {
      display: flex;
      align-items: flex-start;
      gap: 1rem;
      padding: 1rem;
      background: var(--mat-sys-surface-variant);
      border-radius: 8px;
    }

    .shipping-item mat-icon {
      color: var(--mat-sys-primary);
      margin-top: 0.25rem;
    }

    .shipping-item h4 {
      margin: 0 0 0.5rem 0;
      color: var(--mat-sys-on-surface);
    }

    .shipping-item p {
      margin: 0;
      color: var(--mat-sys-on-surface-variant);
    }

    .loading-state,
    .not-found-state {
      display: flex;
      flex-direction: column;
      align-items: center;
      justify-content: center;
      padding: 4rem;
      text-align: center;
      min-height: 400px;
    }

    .not-found-state mat-icon {
      font-size: 4rem;
      width: 4rem;
      height: 4rem;
      opacity: 0.5;
      margin-bottom: 1rem;
    }

    /* Mobile Responsiveness */
    @media (max-width: 768px) {
      .material-detail {
        padding: 1rem;
      }

      .material-header {
        grid-template-columns: 1fr;
        gap: 2rem;
      }

      .main-image {
        height: 300px;
      }

      .action-section {
        flex-direction: column;
      }

      .tab-content {
        padding: 1rem;
      }

      .spec-grid {
        grid-template-columns: 1fr;
      }
    }
  `]
})
export class MaterialDetailComponent implements OnInit {
    private route = inject(ActivatedRoute);
    private router = inject(Router);
    private marketplaceService = inject(MarketplaceService);

    loading = signal(false);
    reviewsLoading = signal(false);
    material = signal<MaterialListing | null>(null);
    reviews = signal<MaterialReview[]>([]);
    selectedImage = signal<string>('');

    ngOnInit() {
        const materialId = this.route.snapshot.paramMap.get('id');
        if (materialId) {
            this.loadMaterial(materialId);
            this.loadReviews(materialId);
        }
    }

    private loadMaterial(id: string) {
        this.loading.set(true);
        this.marketplaceService.getMaterialById(id).subscribe({
            next: (material) => {
                if (material) {
                    this.material.set(material);
                    this.selectedImage.set(material.images[0] || '');
                }
                this.loading.set(false);
            },
            error: () => {
                this.loading.set(false);
            }
        });
    }

    private loadReviews(materialId: string) {
        this.reviewsLoading.set(true);
        this.marketplaceService.getMaterialReviews(materialId).subscribe({
            next: (reviews) => {
                this.reviews.set(reviews);
                this.reviewsLoading.set(false);
            },
            error: () => {
                this.reviewsLoading.set(false);
            }
        });
    }

    selectImage(image: string) {
        this.selectedImage.set(image);
    }

    formatCategoryName(category: string): string {
        return category.replace('_', ' ').replace(/\b\w/g, l => l.toUpperCase());
    }

    getSpecifications(): { key: string; value: any }[] {
        if (!this.material()?.specifications) return [];

        return Object.entries(this.material()!.specifications!).map(([key, value]) => ({
            key: key.charAt(0).toUpperCase() + key.slice(1),
            value: value
        }));
    }

    goBack() {
        this.router.navigate(['/marketplace']);
    }

    requestPurchase() {
        // TODO: Open purchase request dialog
        console.log('Request purchase for material:', this.material()?.id);
    }

    contactSeller() {
        // TODO: Open contact seller dialog
        console.log('Contact seller:', this.material()?.seller.name);
    }
}
