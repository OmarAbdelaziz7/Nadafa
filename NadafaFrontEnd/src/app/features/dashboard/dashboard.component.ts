import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatCardModule } from '@angular/material/card';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { RouterLink } from '@angular/router';

@Component({
    selector: 'app-dashboard',
    standalone: true,
    imports: [
        CommonModule,
        MatCardModule,
        MatButtonModule,
        MatIconModule,
        RouterLink
    ],
    template: `
    <div class="dashboard">
      <div class="dashboard-container">
        <header class="dashboard-header">
          <h1>Welcome to Nadafa</h1>
          <p>Your secure authentication platform is ready to use.</p>
        </header>

        <div class="dashboard-grid">
          <mat-card class="dashboard-card">
            <mat-card-header>
              <mat-icon mat-card-avatar>person</mat-icon>
              <mat-card-title>Profile</mat-card-title>
              <mat-card-subtitle>Manage your account</mat-card-subtitle>
            </mat-card-header>
            <mat-card-content>
              <p>Update your personal information and account preferences.</p>
            </mat-card-content>
            <mat-card-actions>
              <button mat-button routerLink="/profile">VIEW PROFILE</button>
            </mat-card-actions>
          </mat-card>

          <mat-card class="dashboard-card">
            <mat-card-header>
              <mat-icon mat-card-avatar>security</mat-icon>
              <mat-card-title>Security</mat-card-title>
              <mat-card-subtitle>Account security</mat-card-subtitle>
            </mat-card-header>
            <mat-card-content>
              <p>Configure security settings and two-factor authentication.</p>
            </mat-card-content>
            <mat-card-actions>
              <button mat-button routerLink="/security">MANAGE SECURITY</button>
            </mat-card-actions>
          </mat-card>

          <mat-card class="dashboard-card">
            <mat-card-header>
              <mat-icon mat-card-avatar>settings</mat-icon>
              <mat-card-title>Settings</mat-card-title>
              <mat-card-subtitle>Application settings</mat-card-subtitle>
            </mat-card-header>
            <mat-card-content>
              <p>Customize your experience and notification preferences.</p>
            </mat-card-content>
            <mat-card-actions>
              <button mat-button routerLink="/settings">OPEN SETTINGS</button>
            </mat-card-actions>
          </mat-card>

          <mat-card class="dashboard-card">
            <mat-card-header>
              <mat-icon mat-card-avatar>help</mat-icon>
              <mat-card-title>Support</mat-card-title>
              <mat-card-subtitle>Get help</mat-card-subtitle>
            </mat-card-header>
            <mat-card-content>
              <p>Access documentation, FAQs, and contact support.</p>
            </mat-card-content>
            <mat-card-actions>
              <button mat-button>GET HELP</button>
            </mat-card-actions>
          </mat-card>
        </div>
      </div>
    </div>
  `,
    styles: [`
    .dashboard {
      padding: 1rem 1rem 2rem 1rem;
      min-height: calc(100vh - 80px - 200px); /* Account for navbar and footer */
      overflow: visible;
    }

    .dashboard-container {
      max-width: 1200px;
      margin: 0 auto;
    }

    .dashboard-header {
      text-align: center;
      margin: 1rem 0 3rem 0;
      padding: 1rem 0;
    }

    .dashboard-header h1 {
      font-size: 2.5rem;
      font-weight: 700;
      margin: 0 0 1rem 0;
      background: linear-gradient(135deg, var(--mat-sys-primary), var(--mat-sys-tertiary));
      -webkit-background-clip: text;
      -webkit-text-fill-color: transparent;
      background-clip: text;
      line-height: 1.2;
      padding: 0.25rem 0;
      overflow: visible;
      display: block;
    }

    .dashboard-header p {
      font-size: 1.125rem;
      color: var(--mat-sys-on-surface-variant);
      margin: 0;
    }

    .dashboard-grid {
      display: grid;
      grid-template-columns: repeat(auto-fit, minmax(300px, 1fr));
      gap: 1.5rem;
    }

    .dashboard-card {
      transition: transform 0.2s ease, box-shadow 0.2s ease;
    }

    .dashboard-card:hover {
      transform: translateY(-2px);
      box-shadow: 0 8px 24px rgba(0, 0, 0, 0.15);
    }

    .dashboard-card mat-icon[mat-card-avatar] {
      background-color: var(--mat-sys-primary-container);
      color: var(--mat-sys-on-primary-container);
      display: flex !important;
      align-items: center !important;
      justify-content: center !important;
      width: 40px !important;
      height: 40px !important;
      border-radius: 50% !important;
      font-size: 20px !important;
      line-height: 1 !important;
    }

    /* Mobile adjustments */
    @media (max-width: 768px) {
      .dashboard {
        padding: 1rem;
      }

      .dashboard-header h1 {
        font-size: 2rem;
      }

      .dashboard-grid {
        grid-template-columns: 1fr;
      }
    }
  `]
})
export class DashboardComponent { }
