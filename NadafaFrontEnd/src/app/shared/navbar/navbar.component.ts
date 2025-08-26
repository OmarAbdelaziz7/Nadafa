import { Component, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router, RouterLink, RouterLinkActive } from '@angular/router';
import { MatToolbarModule } from '@angular/material/toolbar';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatMenuModule } from '@angular/material/menu';
import { MatDividerModule } from '@angular/material/divider';
import { AuthService } from '../../core/auth/auth.service';

@Component({
    selector: 'app-navbar',
    standalone: true,
    imports: [
        CommonModule,
        RouterLink,
        RouterLinkActive,
        MatToolbarModule,
        MatButtonModule,
        MatIconModule,
        MatMenuModule,
        MatDividerModule
    ],
    template: `
    <mat-toolbar class="navbar">
      <div class="navbar-container">
        <!-- Logo/Brand -->
        <div class="brand">
          <a routerLink="/" class="brand-link">
            <mat-icon class="brand-icon">security</mat-icon>
            <span class="brand-text">Nadafa</span>
          </a>
        </div>

        <!-- Navigation Links (Desktop) -->
        <nav class="nav-links" *ngIf="isLoggedIn()">
          <a class="nav-link" routerLink="/dashboard" routerLinkActive="active">
            <mat-icon>dashboard</mat-icon>
            Dashboard
          </a>
          <a class="nav-link" routerLink="/profile" routerLinkActive="active">
            <mat-icon>person</mat-icon>
            Profile
          </a>
          <a class="nav-link" routerLink="/settings" routerLinkActive="active">
            <mat-icon>settings</mat-icon>
            Settings
          </a>
        </nav>

        <!-- User Menu -->
        <div class="user-section">
          <div *ngIf="isLoggedIn(); else authButtons">
            <button mat-icon-button [matMenuTriggerFor]="userMenu" class="user-menu-trigger">
              <mat-icon>account_circle</mat-icon>
            </button>
            <mat-menu #userMenu="matMenu">
              <button mat-menu-item routerLink="/profile">
                <mat-icon>person</mat-icon>
                <span>Profile</span>
              </button>
              <button mat-menu-item routerLink="/settings">
                <mat-icon>settings</mat-icon>
                <span>Settings</span>
              </button>
              <mat-divider></mat-divider>
              <button mat-menu-item (click)="logout()">
                <mat-icon>logout</mat-icon>
                <span>Logout</span>
              </button>
            </mat-menu>
          </div>

          <ng-template #authButtons>
            <div class="auth-buttons">
              <a mat-button routerLink="/auth/login" class="login-btn">Login</a>
              <a mat-raised-button color="primary" routerLink="/auth/register" class="signup-btn">Sign Up</a>
            </div>
          </ng-template>
        </div>

        <!-- Mobile Menu Button -->
        <button mat-icon-button class="mobile-menu-btn" [matMenuTriggerFor]="mobileMenu">
          <mat-icon>menu</mat-icon>
        </button>
        <mat-menu #mobileMenu="matMenu" class="mobile-menu">
          <div *ngIf="isLoggedIn()">
            <button mat-menu-item routerLink="/dashboard">
              <mat-icon>dashboard</mat-icon>
              <span>Dashboard</span>
            </button>
            <button mat-menu-item routerLink="/profile">
              <mat-icon>person</mat-icon>
              <span>Profile</span>
            </button>
            <button mat-menu-item routerLink="/settings">
              <mat-icon>settings</mat-icon>
              <span>Settings</span>
            </button>
            <mat-divider></mat-divider>
            <button mat-menu-item (click)="logout()">
              <mat-icon>logout</mat-icon>
              <span>Logout</span>
            </button>
          </div>
          <div *ngIf="!isLoggedIn()">
            <button mat-menu-item routerLink="/auth/login">
              <mat-icon>login</mat-icon>
              <span>Login</span>
            </button>
            <button mat-menu-item routerLink="/auth/register">
              <mat-icon>person_add</mat-icon>
              <span>Sign Up</span>
            </button>
          </div>
        </mat-menu>
      </div>
    </mat-toolbar>
  `,
    styles: [`
    .navbar {
      position: fixed;
      top: 0;
      left: 0;
      right: 0;
      z-index: 1000;
      background: rgba(255, 255, 255, 0.95);
      backdrop-filter: blur(10px);
      border-bottom: 1px solid rgba(0, 0, 0, 0.1);
      height: 64px;
    }

    .navbar-container {
      display: flex;
      align-items: center;
      justify-content: space-between;
      width: 100%;
      max-width: 1200px;
      margin: 0 auto;
      padding: 0 1rem;
      height: 100%;
    }

    .brand {
      display: flex;
      align-items: center;
    }

    .brand-link {
      display: flex;
      align-items: center;
      text-decoration: none;
      color: inherit;
      font-weight: 600;
      font-size: 1.25rem;
      gap: 0.5rem;
      transition: color 0.2s ease;
    }

    .brand-link:hover {
      color: var(--mat-sys-primary);
    }

    .brand-icon {
      font-size: 1.5rem;
      width: 1.5rem;
      height: 1.5rem;
      color: var(--mat-sys-primary);
    }

    .brand-text {
      font-family: 'Inter', sans-serif;
      font-weight: 700;
      background: linear-gradient(135deg, var(--mat-sys-primary), var(--mat-sys-tertiary));
      -webkit-background-clip: text;
      -webkit-text-fill-color: transparent;
      background-clip: text;
    }

    .nav-links {
      display: flex;
      align-items: center;
      gap: 0.5rem;
      flex: 1;
      justify-content: center;
    }

    .nav-link {
      display: flex !important;
      align-items: center !important;
      gap: 0.5rem !important;
      font-weight: 500 !important;
      border-radius: 8px !important;
      transition: all 0.2s ease !important;
      color: #374151 !important; /* Force dark color for visibility */
      text-decoration: none !important;
      padding: 0.5rem 1rem !important;
      min-height: 36px !important;
      cursor: pointer !important;
    }

    .nav-link.active {
      background-color: var(--mat-sys-primary-container) !important;
      color: var(--mat-sys-on-primary-container) !important;
    }

    .nav-link:hover {
      background-color: var(--mat-sys-surface-variant) !important;
      color: var(--mat-sys-primary) !important;
    }

    /* Ensure icons in nav links are visible */
    .nav-link mat-icon {
      color: inherit !important;
      font-size: 18px !important;
      width: 18px !important;
      height: 18px !important;
    }

    .user-section {
      display: flex;
      align-items: center;
      gap: 0.5rem;
    }

    .auth-buttons {
      display: flex;
      align-items: center;
      gap: 0.5rem;
    }

    .login-btn {
      font-weight: 500;
    }

    .signup-btn {
      font-weight: 600;
      border-radius: 8px;
    }

    .user-menu-trigger {
      color: var(--mat-sys-primary);
    }

    .mobile-menu-btn {
      display: none;
    }

    /* Mobile Styles */
    @media (max-width: 768px) {
      .nav-links,
      .auth-buttons {
        display: none;
      }

      .mobile-menu-btn {
        display: flex;
      }

      .user-section .user-menu-trigger {
        display: none;
      }
    }

    /* Dark mode support */
    @media (prefers-color-scheme: dark) {
      .navbar {
        background: rgba(18, 18, 18, 0.95);
        border-bottom: 1px solid rgba(255, 255, 255, 0.1);
      }

      .nav-link {
        color: #e5e7eb !important; /* Light text for dark mode */
      }

      .nav-link:hover {
        color: #60a5fa !important; /* Blue for hover in dark mode */
      }
    }
  `]
})
export class NavbarComponent {
    private authService = inject(AuthService);
    private router = inject(Router);

    isLoggedIn = signal(this.authService.isLoggedIn());

    constructor() {
        // Update login status when navigation changes
        this.router.events.subscribe(() => {
            this.isLoggedIn.set(this.authService.isLoggedIn());
        });
    }

    logout() {
        this.authService.logout();
        this.isLoggedIn.set(false);
        this.router.navigate(['/auth/login']);
    }
}
