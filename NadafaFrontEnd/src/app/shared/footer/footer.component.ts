import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';
import { MatIconModule } from '@angular/material/icon';
import { MatButtonModule } from '@angular/material/button';
import { MatDividerModule } from '@angular/material/divider';

@Component({
    selector: 'app-footer',
    standalone: true,
    imports: [
        CommonModule,
        RouterLink,
        MatIconModule,
        MatButtonModule,
        MatDividerModule
    ],
    template: `
    <footer class="footer">
      <div class="footer-container">
        <!-- Main Footer Content -->
        <div class="footer-content">
          <!-- Brand Section -->
          <div class="footer-section brand-section">
            <div class="footer-brand">
              <mat-icon class="brand-icon">security</mat-icon>
              <span class="brand-text">Nadafa</span>
            </div>
            <p class="brand-description">
              Secure and reliable authentication platform designed to protect your digital identity.
            </p>
            <div class="social-links">
              <a href="#" mat-icon-button aria-label="Twitter">
                <mat-icon>twitter</mat-icon>
              </a>
              <a href="#" mat-icon-button aria-label="LinkedIn">
                <mat-icon>linkedin</mat-icon>
              </a>
              <a href="#" mat-icon-button aria-label="GitHub">
                <mat-icon>code</mat-icon>
              </a>
              <a href="#" mat-icon-button aria-label="Email">
                <mat-icon>email</mat-icon>
              </a>
            </div>
          </div>

          <!-- Quick Links -->
          <div class="footer-section">
            <h4 class="section-title">Platform</h4>
            <ul class="footer-links">
              <li><a routerLink="/dashboard">Dashboard</a></li>
              <li><a routerLink="/profile">Profile</a></li>
              <li><a routerLink="/settings">Settings</a></li>
              <li><a routerLink="/security">Security</a></li>
            </ul>
          </div>

          <!-- Resources -->
          <div class="footer-section">
            <h4 class="section-title">Resources</h4>
            <ul class="footer-links">
              <li><a href="#" target="_blank">Documentation</a></li>
              <li><a href="#" target="_blank">API Reference</a></li>
              <li><a href="#" target="_blank">Help Center</a></li>
              <li><a href="#" target="_blank">Status Page</a></li>
            </ul>
          </div>

          <!-- Legal -->
          <div class="footer-section">
            <h4 class="section-title">Legal</h4>
            <ul class="footer-links">
              <li><a href="#" target="_blank">Privacy Policy</a></li>
              <li><a href="#" target="_blank">Terms of Service</a></li>
              <li><a href="#" target="_blank">Cookie Policy</a></li>
              <li><a href="#" target="_blank">Security</a></li>
            </ul>
          </div>

          <!-- Support -->
          <div class="footer-section">
            <h4 class="section-title">Support</h4>
            <ul class="footer-links">
              <li><a href="#" target="_blank">Contact Us</a></li>
              <li><a href="#" target="_blank">Community</a></li>
              <li><a href="#" target="_blank">Bug Reports</a></li>
              <li><a href="#" target="_blank">Feature Requests</a></li>
            </ul>
          </div>
        </div>

        <mat-divider></mat-divider>

        <!-- Footer Bottom -->
        <div class="footer-bottom">
          <div class="copyright">
            <p>&copy; {{ currentYear }} Nadafa. All rights reserved.</p>
          </div>
          <div class="footer-meta">
            <span class="version">v1.0.0</span>
            <span class="divider">•</span>
            <span class="status">
              <mat-icon class="status-icon online">circle</mat-icon>
              All systems operational
            </span>
          </div>
        </div>
      </div>
    </footer>
  `,
    styles: [`
    .footer {
      background: var(--mat-sys-surface-container);
      border-top: 1px solid var(--mat-sys-outline-variant);
      margin-top: auto;
      padding: 3rem 0 1.5rem;
    }

    .footer-container {
      max-width: 1200px;
      margin: 0 auto;
      padding: 0 1rem;
    }

    .footer-content {
      display: grid;
      grid-template-columns: 2fr 1fr 1fr 1fr 1fr;
      gap: 2rem;
      margin-bottom: 2rem;
    }

    .footer-section {
      display: flex;
      flex-direction: column;
      gap: 1rem;
    }

    .brand-section {
      max-width: 300px;
    }

    .footer-brand {
      display: flex;
      align-items: center;
      gap: 0.5rem;
      margin-bottom: 1rem;
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
      font-size: 1.25rem;
      background: linear-gradient(135deg, var(--mat-sys-primary), var(--mat-sys-tertiary));
      -webkit-background-clip: text;
      -webkit-text-fill-color: transparent;
      background-clip: text;
    }

    .brand-description {
      color: var(--mat-sys-on-surface-variant);
      line-height: 1.6;
      margin: 0 0 1rem 0;
      font-size: 0.875rem;
    }

    .social-links {
      display: flex;
      gap: 0.5rem;
    }

    .social-links a {
      color: var(--mat-sys-on-surface-variant);
      transition: color 0.2s ease;
    }

    .social-links a:hover {
      color: var(--mat-sys-primary);
    }

    .section-title {
      font-weight: 600;
      font-size: 0.875rem;
      color: var(--mat-sys-on-surface);
      margin: 0 0 0.5rem 0;
      text-transform: uppercase;
      letter-spacing: 0.5px;
    }

    .footer-links {
      list-style: none;
      padding: 0;
      margin: 0;
      display: flex;
      flex-direction: column;
      gap: 0.75rem;
    }

    .footer-links a {
      color: var(--mat-sys-on-surface-variant);
      text-decoration: none;
      font-size: 0.875rem;
      transition: color 0.2s ease;
      line-height: 1.4;
    }

    .footer-links a:hover {
      color: var(--mat-sys-primary);
    }

    .footer-bottom {
      display: flex;
      justify-content: space-between;
      align-items: center;
      padding-top: 1.5rem;
      font-size: 0.75rem;
      color: var(--mat-sys-on-surface-variant);
    }

    .copyright p {
      margin: 0;
    }

    .footer-meta {
      display: flex;
      align-items: center;
      gap: 0.5rem;
    }

    .divider {
      opacity: 0.5;
    }

    .status {
      display: flex;
      align-items: center;
      gap: 0.25rem;
    }

    .status-icon {
      font-size: 0.5rem;
      width: 0.5rem;
      height: 0.5rem;
    }

    .status-icon.online {
      color: #10b981;
    }

    /* Tablet Styles */
    @media (max-width: 1024px) {
      .footer-content {
        grid-template-columns: 2fr 1fr 1fr;
        gap: 1.5rem;
      }

      .footer-section:nth-child(4),
      .footer-section:nth-child(5) {
        grid-column: 2 / -1;
        display: grid;
        grid-template-columns: 1fr 1fr;
        gap: 1.5rem;
      }
    }

    /* Mobile Styles */
    @media (max-width: 768px) {
      .footer {
        padding: 2rem 0 1rem;
      }

      .footer-content {
        grid-template-columns: 1fr;
        gap: 2rem;
      }

      .footer-section:nth-child(4),
      .footer-section:nth-child(5) {
        grid-column: 1;
        display: block;
      }

      .footer-bottom {
        flex-direction: column;
        gap: 1rem;
        text-align: center;
      }

      .footer-meta {
        justify-content: center;
      }
    }

    /* Dark mode support */
    @media (prefers-color-scheme: dark) {
      .status-icon.online {
        color: #34d399;
      }
    }
  `]
})
export class FooterComponent {
    currentYear = new Date().getFullYear();
}
