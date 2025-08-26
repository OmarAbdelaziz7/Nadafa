import { Component, OnInit, inject, signal, computed } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';
import { FormsModule, ReactiveFormsModule, FormBuilder, FormGroup } from '@angular/forms';

// Material imports
import { MatCardModule } from '@angular/material/card';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { MatChipsModule } from '@angular/material/chips';
import { MatSliderModule } from '@angular/material/slider';
import { MatCheckboxModule } from '@angular/material/checkbox';
import { MatExpansionModule } from '@angular/material/expansion';
import { MatPaginatorModule, PageEvent } from '@angular/material/paginator';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatBadgeModule } from '@angular/material/badge';
import { MatMenuModule } from '@angular/material/menu';
import { MatDividerModule } from '@angular/material/divider';
import { MatTooltipModule } from '@angular/material/tooltip';

import { MarketplaceService } from '../../core/services/marketplace.service';
import {
    MaterialListing,
    MaterialCategory,
    MarketplaceFilters,
    MarketplaceSortOptions
} from '../../shared/models/marketplace.models';

@Component({
    selector: 'app-marketplace',
    standalone: true,
    imports: [
        CommonModule,
        FormsModule,
        ReactiveFormsModule,

        MatCardModule,
        MatButtonModule,
        MatIconModule,
        MatFormFieldModule,
        MatInputModule,
        MatSelectModule,
        MatChipsModule,
        MatSliderModule,
        MatCheckboxModule,
        MatExpansionModule,
        MatPaginatorModule,
        MatProgressSpinnerModule,
        MatBadgeModule,
        MatMenuModule,
        MatDividerModule,
        MatTooltipModule
    ],
    template: `
    <div class="marketplace">
      <div class="marketplace-container">
        <!-- Header -->
        <div class="marketplace-header">
          <h1>Recyclable Materials Marketplace</h1>
          <p>Browse and purchase recyclable materials from verified sellers</p>
          
          <div class="header-stats">
            <div class="stat">
              <mat-icon>inventory</mat-icon>
              <span class="number">{{ filteredMaterials().length }}</span>
              <span class="label">Available Materials</span>
            </div>
            <div class="stat">
              <mat-icon>store</mat-icon>
              <span class="number">{{ uniqueSellers().length }}</span>
              <span class="label">Active Sellers</span>
            </div>
            <div class="stat">
              <mat-icon>category</mat-icon>
              <span class="number">{{ availableCategories().length }}</span>
              <span class="label">Categories</span>
            </div>
          </div>
        </div>

        <div class="marketplace-content">
          <!-- Filters Sidebar -->
          <aside class="filters-sidebar">
            <mat-card class="filters-card">
              <mat-card-header>
                <mat-card-title>
                  <mat-icon>filter_list</mat-icon>
                  Filters
                </mat-card-title>
                <button mat-icon-button (click)="clearFilters()" matTooltip="Clear all filters">
                  <mat-icon>clear</mat-icon>
                </button>
              </mat-card-header>

              <mat-card-content>
                <form [formGroup]="filtersForm">
                  <!-- Search -->
                  <mat-form-field appearance="outline" class="full-width">
                    <mat-label>Search materials or sellers</mat-label>
                    <input matInput formControlName="searchTerm" placeholder="e.g., plastic bottles">
                    <mat-icon matSuffix>search</mat-icon>
                  </mat-form-field>

                  <!-- Categories -->
                  <mat-expansion-panel>
                    <mat-expansion-panel-header>
                      <mat-panel-title>Categories</mat-panel-title>
                      <mat-panel-description *ngIf="selectedCategories().length">
                        {{ selectedCategories().length }} selected
                      </mat-panel-description>
                    </mat-expansion-panel-header>
                    
                    <div class="checkbox-group">
                      <mat-checkbox 
                        *ngFor="let category of allCategories" 
                        [checked]="selectedCategories().includes(category)"
                        (change)="toggleCategory(category)">
                        {{ formatCategoryName(category) }}
                      </mat-checkbox>
                    </div>
                  </mat-expansion-panel>

                  <!-- Price Range -->
                  <mat-expansion-panel>
                    <mat-expansion-panel-header>
                      <mat-panel-title>Price per Unit</mat-panel-title>
                      <mat-panel-description>
                        {{ getMinPrice() }} - {{ getMaxPrice() }}
                      </mat-panel-description>
                    </mat-expansion-panel-header>
                    
                    <div class="price-range">
                      <mat-form-field appearance="outline">
                        <mat-label>Min Price</mat-label>
                        <input matInput type="number" formControlName="minPrice" min="0">
                        <span matTextPrefix>$</span>
                      </mat-form-field>
                      
                      <mat-form-field appearance="outline">
                        <mat-label>Max Price</mat-label>
                        <input matInput type="number" formControlName="maxPrice" min="0">
                        <span matTextPrefix>$</span>
                      </mat-form-field>
                    </div>
                  </mat-expansion-panel>

                  <!-- Quantity Range -->
                  <mat-expansion-panel>
                    <mat-expansion-panel-header>
                      <mat-panel-title>Quantity</mat-panel-title>
                      <mat-panel-description>
                        {{ getMinQuantity() }}+ units
                      </mat-panel-description>
                    </mat-expansion-panel-header>
                    
                    <mat-form-field appearance="outline" class="full-width">
                      <mat-label>Minimum Quantity</mat-label>
                      <input matInput type="number" formControlName="minQuantity" min="0">
                    </mat-form-field>
                  </mat-expansion-panel>

                  

                  <!-- Location -->
                  <mat-expansion-panel>
                    <mat-expansion-panel-header>
                      <mat-panel-title>Location</mat-panel-title>
                    </mat-expansion-panel-header>
                    
                    <div class="location-filters">
                      <mat-form-field appearance="outline" class="full-width">
                        <mat-label>City</mat-label>
                        <input matInput formControlName="city" placeholder="e.g., Austin">
                      </mat-form-field>
                      
                      <mat-form-field appearance="outline" class="full-width">
                        <mat-label>State</mat-label>
                        <input matInput formControlName="state" placeholder="e.g., Texas">
                      </mat-form-field>
                    </div>
                  </mat-expansion-panel>

                  <!-- Seller Rating -->
                  
                </form>
              </mat-card-content>
            </mat-card>
          </aside>

          <!-- Materials Grid -->
          <main class="materials-main">
            <!-- Toolbar -->
            <div class="materials-toolbar">
              <div class="results-info">
                <span class="results-count">{{ filteredMaterials().length }} materials found</span>
                <span class="applied-filters" *ngIf="hasActiveFilters()">
                  {{ getActiveFiltersCount() }} filters applied
                  <button mat-button (click)="clearFilters()">Clear all</button>
                </span>
              </div>

              <div class="sort-controls">
                <mat-form-field appearance="outline">
                  <mat-label>Sort by</mat-label>
                  <mat-select [value]="currentSort().field + '_' + currentSort().direction" 
                            (selectionChange)="onSortChange($event.value)">
                    <mat-option value="date_desc">Newest first</mat-option>
                    <mat-option value="date_asc">Oldest first</mat-option>
                    <mat-option value="price_asc">Price: Low to High</mat-option>
                    <mat-option value="price_desc">Price: High to Low</mat-option>
                    <mat-option value="quantity_desc">Quantity: High to Low</mat-option>
                    <mat-option value="rating_desc">Highest rated sellers</mat-option>
                  </mat-select>
                </mat-form-field>

                <button mat-icon-button [matMenuTriggerFor]="viewMenu" matTooltip="View options">
                  <mat-icon>view_module</mat-icon>
                </button>
                <mat-menu #viewMenu="matMenu">
                  <button mat-menu-item (click)="setViewMode('grid')">
                    <mat-icon>view_module</mat-icon>
                    <span>Grid view</span>
                  </button>
                  <button mat-menu-item (click)="setViewMode('list')">
                    <mat-icon>view_list</mat-icon>
                    <span>List view</span>
                  </button>
                </mat-menu>
              </div>
            </div>

            <!-- Loading State -->
            <div *ngIf="loading()" class="loading-state">
              <mat-progress-spinner mode="indeterminate" diameter="50"></mat-progress-spinner>
              <p>Loading materials...</p>
            </div>

            <!-- Materials Grid/List -->
            <div *ngIf="!loading()" class="materials-grid" [class.list-view]="viewMode() === 'list'">
              <div *ngFor="let material of paginatedMaterials()" class="material-card-wrapper">
                <mat-card class="material-card" [class.list-card]="viewMode() === 'list'">
                  <!-- Material Image -->
                  <div class="material-image" [style.background-image]="'url(' + (material.images[0] || '/placeholder.jpg') + ')'">
                    <div class="material-badges">
                      <span class="category-badge">{{ formatCategoryName(material.category) }}</span>
                    </div>
                  </div>

                  <mat-card-content>
                    <!-- Material Info -->
                    <div class="material-info">
                      <h3 class="material-name">{{ material.name }}</h3>
                      <p class="material-description">{{ material.description }}</p>
                      
                      <div class="material-specs">
                        <div class="spec">
                          <mat-icon>inventory</mat-icon>
                          <span>{{ material.quantity }} {{ material.unit }}</span>
                        </div>
                        <div class="spec">
                          <mat-icon>attach_money</mat-icon>
                          <span>\${{ material.pricePerUnit }}/{{ material.unit }}</span>
                        </div>
                        <div class="spec">
                          <mat-icon>location_on</mat-icon>
                          <span>{{ material.location.city }}, {{ material.location.state }}</span>
                        </div>
                      </div>

                      <!-- Shipping Information -->
                      <div class="shipping-info">
                        <mat-chip>Free Shipping Included</mat-chip>
                      </div>
                    </div>

                    <!-- Seller Info -->
                    <div class="seller-info">
                      <div class="seller-avatar">
                        <img [src]="material.seller.profileImage" [alt]="material.seller.name">
                        <mat-icon *ngIf="material.seller.verified" class="verified-badge" matTooltip="Verified seller">verified</mat-icon>
                      </div>
                      <div class="seller-details">
                        <span class="seller-name">{{ material.seller.name }}</span>
                        <div class="seller-verification">
                          <mat-icon *ngIf="material.seller.verified" class="verified-icon">verified</mat-icon>
                          <span *ngIf="material.seller.verified" class="verified-text">Verified Seller</span>
                          <span *ngIf="!material.seller.verified" class="unverified-text">Unverified</span>
                        </div>
                      </div>
                    </div>
                  </mat-card-content>

                  <mat-card-actions>
                    <div class="card-actions">
                      <div class="price-info">
                        <span class="total-price">\${{ material.totalPrice }}</span>
                        <span class="price-label">Total</span>
                      </div>
                      <div class="action-buttons">
                        <button mat-button (click)="viewMaterialDetails(material.id)">
                          <mat-icon>visibility</mat-icon>
                          View Details
                        </button>
                        <button mat-raised-button color="primary" (click)="requestPurchase(material)">
                          <mat-icon>shopping_cart</mat-icon>
                          Request Purchase
                        </button>
                      </div>
                    </div>
                  </mat-card-actions>
                </mat-card>
              </div>
            </div>

            <!-- Empty State -->
            <div *ngIf="!loading() && filteredMaterials().length === 0" class="empty-state">
              <mat-icon>search_off</mat-icon>
              <h3>No materials found</h3>
              <p>Try adjusting your filters or search terms.</p>
              <button mat-raised-button (click)="clearFilters()">Clear Filters</button>
            </div>

            <!-- Pagination -->
            <mat-paginator 
              *ngIf="!loading() && filteredMaterials().length > 0"
              [length]="filteredMaterials().length"
              [pageSize]="pageSize()"
              [pageSizeOptions]="[6, 12, 24, 48]"
              [pageIndex]="currentPage()"
              (page)="onPageChange($event)"
              showFirstLastButtons>
            </mat-paginator>
          </main>
        </div>
      </div>
    </div>
  `,
    styles: [`
    .marketplace {
      min-height: calc(100vh - 80px);
      background: #f5f5f5;
      padding: 1rem;
    }

    .marketplace-container {
      max-width: 1400px;
      margin: 0 auto;
    }

    .marketplace-header {
      text-align: center;
      margin-bottom: 2rem;
      background: white;
      padding: 3rem 2rem;
      border-radius: 12px;
      box-shadow: 0 2px 8px rgba(0,0,0,0.1);
      overflow: visible;
    }

    .marketplace-header h1 {
      font-size: 2.5rem;
      font-weight: 700;
      margin: 0 0 0.5rem 0;
      background: linear-gradient(135deg, var(--mat-sys-primary), var(--mat-sys-tertiary));
      -webkit-background-clip: text;
      -webkit-text-fill-color: transparent;
      background-clip: text;
      line-height: 1.2;
      padding: 0.5rem 0;
      overflow: visible;
      display: block;
    }

    .marketplace-header p {
      font-size: 1.125rem;
      color: var(--mat-sys-on-surface-variant);
      margin: 0 0 2rem 0;
    }

    .header-stats {
      display: flex;
      justify-content: center;
      gap: 2rem;
      flex-wrap: wrap;
    }

    .stat {
      display: flex;
      flex-direction: column;
      align-items: center;
      gap: 0.5rem;
      padding: 1rem;
      background: var(--mat-sys-surface-variant);
      border-radius: 8px;
      min-width: 120px;
    }

    .stat mat-icon {
      color: var(--mat-sys-primary);
      font-size: 2rem;
      width: 2rem;
      height: 2rem;
    }

    .stat .number {
      font-size: 1.5rem;
      font-weight: 700;
      color: var(--mat-sys-on-surface);
    }

    .stat .label {
      font-size: 0.875rem;
      color: var(--mat-sys-on-surface-variant);
    }

    .marketplace-content {
      display: grid;
      grid-template-columns: 320px 1fr;
      gap: 2rem;
      align-items: start;
    }

    .filters-sidebar {
      position: sticky;
      top: 1rem;
    }

    .filters-card {
      padding: 0;
      overflow: visible;
    }

    .filters-card mat-card-header {
      padding: 1rem;
      background: var(--mat-sys-surface-variant);
    }

    .filters-card mat-card-content {
      padding: 1.5rem 1rem;
    }

    .full-width {
      width: 100%;
      margin-bottom: 1.5rem;
    }

    /* Expansion panel spacing */
    .filters-card mat-expansion-panel {
      margin-bottom: 1rem !important;
    }

    .filters-card mat-expansion-panel:last-child {
      margin-bottom: 0 !important;
    }

    .checkbox-group {
      display: flex;
      flex-direction: column;
      gap: 0.75rem;
      margin: 1.5rem 0;
      padding: 0.5rem 0;
    }

    .price-range {
      display: grid;
      grid-template-columns: 1fr 1fr;
      gap: 1rem;
      margin: 1.5rem 0;
      padding: 0.5rem 0;
    }

    .location-filters {
      margin: 1.5rem 0;
      padding: 0.5rem 0;
    }

    .materials-main {
      min-height: 600px;
    }

    .materials-toolbar {
      display: flex;
      justify-content: space-between;
      align-items: center;
      margin-bottom: 1.5rem;
      padding: 1rem;
      background: white;
      border-radius: 8px;
      box-shadow: 0 2px 4px rgba(0,0,0,0.1);
    }

    .results-info {
      display: flex;
      flex-direction: column;
      gap: 0.25rem;
    }

    .results-count {
      font-weight: 600;
      color: var(--mat-sys-on-surface);
    }

    .applied-filters {
      font-size: 0.875rem;
      color: var(--mat-sys-on-surface-variant);
    }

    .sort-controls {
      display: flex;
      align-items: center;
      gap: 1rem;
    }

    .loading-state {
      display: flex;
      flex-direction: column;
      align-items: center;
      justify-content: center;
      padding: 4rem;
      text-align: center;
    }

    .materials-grid {
      display: grid;
      grid-template-columns: repeat(auto-fill, minmax(400px, 1fr));
      gap: 1.5rem;
    }

    .materials-grid.list-view {
      grid-template-columns: 1fr;
    }

    .material-card {
      height: 100%;
      display: flex;
      flex-direction: column;
      transition: transform 0.2s ease, box-shadow 0.2s ease;
    }

    .material-card:hover {
      transform: translateY(-2px);
      box-shadow: 0 8px 24px rgba(0,0,0,0.15);
    }

    .material-card.list-card {
      display: grid;
      grid-template-columns: 200px 1fr auto;
      align-items: center;
      height: auto;
    }

    .material-image {
      height: 200px;
      background-size: cover;
      background-position: center;
      background-color: #f0f0f0;
      position: relative;
      border-radius: 8px 8px 0 0;
    }

    .list-card .material-image {
      height: 120px;
      border-radius: 8px;
      margin: 1rem;
    }

    .material-badges {
      position: absolute;
      top: 0.5rem;
      right: 0.5rem;
      display: flex;
      flex-direction: column;
      gap: 0.25rem;
    }

    .category-badge {
      padding: 0.25rem 0.5rem;
      border-radius: 4px;
      font-size: 0.75rem;
      font-weight: 600;
      text-transform: uppercase;
      background: rgba(33, 150, 243, 0.9);
      color: white;
    }

    .material-info {
      flex: 1;
      padding: 1rem;
    }

    .material-name {
      font-size: 1.25rem;
      font-weight: 600;
      margin: 0 0 0.5rem 0;
      color: var(--mat-sys-on-surface);
    }

    .material-description {
      color: var(--mat-sys-on-surface-variant);
      margin: 0 0 1rem 0;
      line-height: 1.5;
      display: -webkit-box;
      -webkit-line-clamp: 2;
      -webkit-box-orient: vertical;
      overflow: hidden;
    }

    .material-specs {
      display: flex;
      flex-wrap: wrap;
      gap: 1rem;
      margin-bottom: 1rem;
    }

    .spec {
      display: flex;
      align-items: center;
      gap: 0.25rem;
      font-size: 0.875rem;
      color: var(--mat-sys-on-surface-variant);
    }

    .spec mat-icon {
      font-size: 1rem;
      width: 1rem;
      height: 1rem;
    }

    .shipping-info {
      margin-top: 1rem;
    }

    .shipping-info mat-chip {
      font-size: 0.75rem;
      background: var(--mat-sys-primary-container);
      color: var(--mat-sys-on-primary-container);
    }

    .seller-info {
      display: flex;
      align-items: center;
      gap: 0.75rem;
      padding: 1rem;
      border-top: 1px solid var(--mat-sys-outline-variant);
      background: var(--mat-sys-surface-variant);
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

    .seller-details {
      flex: 1;
    }

    .seller-name {
      font-weight: 600;
      color: var(--mat-sys-on-surface);
      display: block;
      margin-bottom: 0.25rem;
    }

    .seller-verification {
      display: flex;
      align-items: center;
      gap: 0.25rem;
      font-size: 0.875rem;
    }

    .verified-icon {
      color: var(--mat-sys-primary);
      font-size: 1rem;
      width: 1rem;
      height: 1rem;
    }

    .verified-text {
      color: var(--mat-sys-primary);
      font-weight: 500;
    }

    .unverified-text {
      color: var(--mat-sys-on-surface-variant);
    }

    .seller-rating {
      display: flex;
      align-items: center;
      gap: 0.5rem;
      font-size: 0.875rem;
    }



    .card-actions {
      display: flex;
      justify-content: space-between;
      align-items: center;
      padding: 1rem;
      border-top: 1px solid var(--mat-sys-outline-variant);
    }

    .price-info {
      display: flex;
      flex-direction: column;
    }

    .total-price {
      font-size: 1.5rem;
      font-weight: 700;
      color: var(--mat-sys-primary);
    }

    .price-label {
      font-size: 0.875rem;
      color: var(--mat-sys-on-surface-variant);
    }

    .action-buttons {
      display: flex;
      gap: 0.5rem;
    }

    .empty-state {
      text-align: center;
      padding: 4rem;
      color: var(--mat-sys-on-surface-variant);
    }

    .empty-state mat-icon {
      font-size: 4rem;
      width: 4rem;
      height: 4rem;
      opacity: 0.5;
      margin-bottom: 1rem;
    }

    .empty-state h3 {
      margin: 0 0 0.5rem 0;
      color: var(--mat-sys-on-surface);
    }

    /* Mobile Responsiveness */
    @media (max-width: 768px) {
      .marketplace-content {
        grid-template-columns: 1fr;
      }

      .filters-sidebar {
        position: static;
      }

      .materials-grid {
        grid-template-columns: 1fr;
      }

      .materials-toolbar {
        flex-direction: column;
        gap: 1rem;
        align-items: stretch;
      }

      .sort-controls {
        justify-content: space-between;
      }

      .header-stats {
        gap: 1rem;
      }

      .stat {
        min-width: 100px;
        padding: 0.75rem;
      }

      .card-actions {
        flex-direction: column;
        gap: 1rem;
        align-items: stretch;
      }

      .action-buttons {
        justify-content: space-between;
      }
    }
  `]
})
export class MarketplaceComponent implements OnInit {
    private marketplaceService = inject(MarketplaceService);
    private fb = inject(FormBuilder);
    private router = inject(Router);

    // Signals
    loading = signal(false);
    materials = signal<MaterialListing[]>([]);
    viewMode = signal<'grid' | 'list'>('grid');
    currentPage = signal(0);
    pageSize = signal(12);

    // Form
    filtersForm: FormGroup;

    // Static data
    allCategories = this.marketplaceService.getCategories();

    // Computed values
    selectedCategories = signal<MaterialCategory[]>([]);
    currentSort = signal<MarketplaceSortOptions>({ field: 'date', direction: 'desc' });

    filteredMaterials = computed(() => {
        return this.materials();
    });

    paginatedMaterials = computed(() => {
        const start = this.currentPage() * this.pageSize();
        const end = start + this.pageSize();
        return this.filteredMaterials().slice(start, end);
    });

    uniqueSellers = computed(() => {
        const sellers = this.materials().map(m => m.seller);
        return sellers.filter((seller, index, self) =>
            index === self.findIndex(s => s.id === seller.id)
        );
    });

    availableCategories = computed(() => {
        const categories = [...new Set(this.materials().map(m => m.category))];
        return categories;
    });

    constructor() {
        this.filtersForm = this.fb.group({
            searchTerm: [''],
            minPrice: [null],
            maxPrice: [null],
            minQuantity: [null],
            city: [''],
            state: ['']
        });

        // Watch form changes
        this.filtersForm.valueChanges.subscribe(() => {
            this.applyFilters();
        });
    }

    ngOnInit() {
        this.loadMaterials();
    }

    loadMaterials() {
        this.loading.set(true);

        const filters = this.buildFilters();
        const sort = this.currentSort();

        this.marketplaceService.getMaterialListings(filters, sort).subscribe({
            next: (materials) => {
                this.materials.set(materials);
                this.loading.set(false);
            },
            error: (error) => {
                console.error('Error loading materials:', error);
                this.loading.set(false);
            }
        });
    }

    private buildFilters(): MarketplaceFilters {
        const formValue = this.filtersForm.value;

        return {
            searchTerm: formValue.searchTerm || undefined,
            categories: this.selectedCategories().length ? this.selectedCategories() : undefined,
            minPrice: formValue.minPrice || undefined,
            maxPrice: formValue.maxPrice || undefined,
            minQuantity: formValue.minQuantity || undefined,

            location: {
                city: formValue.city || undefined,
                state: formValue.state || undefined
            }
        };
    }

    applyFilters() {
        this.currentPage.set(0);
        this.loadMaterials();
    }

    toggleCategory(category: MaterialCategory) {
        const current = this.selectedCategories();
        if (current.includes(category)) {
            this.selectedCategories.set(current.filter(c => c !== category));
        } else {
            this.selectedCategories.set([...current, category]);
        }
        this.applyFilters();
    }



    clearFilters() {
        this.filtersForm.reset({
            searchTerm: '',
            minPrice: null,
            maxPrice: null,
            minQuantity: null,
            city: '',
            state: ''
        });
        this.selectedCategories.set([]);
        this.applyFilters();
    }

    hasActiveFilters(): boolean {
        const formValue = this.filtersForm.value;
        return !!(
            formValue.searchTerm ||
            formValue.minPrice ||
            formValue.maxPrice ||
            formValue.minQuantity ||
            formValue.city ||
            formValue.state ||
            this.selectedCategories().length
        );
    }

    getActiveFiltersCount(): number {
        let count = 0;
        const formValue = this.filtersForm.value;

        if (formValue.searchTerm) count++;
        if (formValue.minPrice) count++;
        if (formValue.maxPrice) count++;
        if (formValue.minQuantity) count++;
        if (formValue.city) count++;
        if (formValue.state) count++;
        count += this.selectedCategories().length;

        return count;
    }

    onSortChange(value: string) {
        const [field, direction] = value.split('_') as [any, 'asc' | 'desc'];
        this.currentSort.set({ field, direction });
        this.applyFilters();
    }

    setViewMode(mode: 'grid' | 'list') {
        this.viewMode.set(mode);
    }

    onPageChange(event: PageEvent) {
        this.currentPage.set(event.pageIndex);
        this.pageSize.set(event.pageSize);
    }

    formatCategoryName(category: MaterialCategory): string {
        return category.replace('_', ' ').replace(/\b\w/g, l => l.toUpperCase());
    }





    viewMaterialDetails(materialId: string) {
        this.router.navigate(['/marketplace', materialId]);
    }

    requestPurchase(material: MaterialListing) {
        // TODO: Open purchase request dialog
        console.log('Request purchase:', material);
    }

    // Getter methods for template
    getMinPrice(): number {
        return this.filtersForm.get('minPrice')?.value || 0;
    }

    getMaxPrice(): number {
        return this.filtersForm.get('maxPrice')?.value || 20;
    }

    getMinQuantity(): number {
        return this.filtersForm.get('minQuantity')?.value || 0;
    }


}
