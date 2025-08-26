import { Component, inject, signal } from '@angular/core';
import { ReactiveFormsModule, FormBuilder, Validators } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { MatCardModule } from '@angular/material/card';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatSnackBar, MatSnackBarModule } from '@angular/material/snack-bar';
import { MatChipsModule } from '@angular/material/chips';
import { AuthService } from '../../../core/auth/auth.service';
import { NgIf } from '@angular/common';

@Component({
  standalone: true,
  selector: 'app-login',
  imports: [
    ReactiveFormsModule, RouterLink, NgIf,
    MatCardModule, MatFormFieldModule, MatInputModule,
    MatButtonModule, MatIconModule, MatSnackBarModule, MatChipsModule,
  ],
  styles: [`
    .card { padding: 1rem; }
    .actions { display:flex; gap:.75rem; align-items:center; justify-content:space-between; margin-top:.5rem; }
    .links { display:flex; gap:.75rem; justify-content:center; margin-top:.75rem; }
    .dev-info { 
      margin: 1rem 0; 
      padding: 1rem; 
      background: #e3f2fd; 
      border-radius: 8px; 
      border-left: 4px solid #2196f3;
    }
    .dev-info h4 { 
      margin: 0 0 0.5rem 0; 
      color: #1976d2; 
      font-size: 0.875rem;
      font-weight: 600;
    }
    .dev-info p { 
      margin: 0.25rem 0; 
      font-size: 0.75rem; 
      color: #424242;
    }
    .dev-credentials { 
      font-family: monospace; 
      font-weight: 600;
      color: #1976d2;
    }
    .dev-fill-btn {
      margin-top: 0.5rem;
      font-size: 0.75rem;
    }
  `],
  template: `
    <mat-card class="card">
      <!-- Development Mode Indicator -->
      <div *ngIf="devCredentials" class="dev-info">
        <h4>🚀 Development Mode</h4>
        <p>Use these credentials to bypass authentication:</p>
        <p class="dev-credentials">Email: {{ devCredentials.email }}</p>
        <p class="dev-credentials">Password: {{ devCredentials.password }}</p>
        <div style="display: flex; gap: 0.5rem; margin-top: 0.5rem;">
          <button mat-stroked-button 
                  color="primary" 
                  class="dev-fill-btn" 
                  type="button"
                  (click)="fillDevCredentials()">
            <mat-icon style="font-size: 16px; width: 16px; height: 16px;">flash_on</mat-icon>
            Quick Fill
          </button>
          <button mat-flat-button 
                  color="accent" 
                  class="dev-fill-btn" 
                  type="button"
                  (click)="skipAuth()">
            <mat-icon style="font-size: 16px; width: 16px; height: 16px;">skip_next</mat-icon>
            Skip Auth
          </button>
        </div>
      </div>

      <form [formGroup]="form" (ngSubmit)="onSubmit()">
        <mat-form-field appearance="outline" class="w-full">
          <mat-label>Email</mat-label>
          <input matInput type="email" formControlName="email" autocomplete="email" required />
        </mat-form-field>

        <mat-form-field appearance="outline" class="w-full">
          <mat-label>Password</mat-label>
          <input matInput [type]="hide() ? 'password' : 'text'" formControlName="password" autocomplete="current-password" required />
          <button mat-icon-button matSuffix type="button" (click)="toggleHide()" [attr.aria-label]="hide() ? 'Show' : 'Hide'">
            <mat-icon>{{ hide() ? 'visibility' : 'visibility_off' }}</mat-icon>
          </button>
        </mat-form-field>

        <div class="actions">
          <button mat-flat-button color="primary" type="submit" [disabled]="form.invalid || loading()">{{ loading() ? 'Signing in…' : 'Sign in' }}</button>
          <a routerLink="/auth/forgot-password">Forgot password?</a>
        </div>

        <div class="links">
          <span>New here?</span> <a routerLink="/auth/register">Create account</a>
        </div>
      </form>
    </mat-card>
  `,
})
export class LoginComponent {
  private fb = inject(FormBuilder);
  private auth = inject(AuthService);
  private router = inject(Router);
  private snack = inject(MatSnackBar);

  hide = signal(true);
  loading = signal(false);
  devCredentials = this.auth.getDevCredentials();

  toggleHide() { this.hide.set(!this.hide()); }

  form = this.fb.nonNullable.group({
    email: ['', [Validators.required, Validators.email]],
    password: ['', [Validators.required, Validators.minLength(6)]],
  });

  fillDevCredentials() {
    if (this.devCredentials) {
      this.form.patchValue({
        email: this.devCredentials.email,
        password: this.devCredentials.password
      });
      this.snack.open('🚀 Development credentials filled!', 'OK', { duration: 1500 });
    }
  }

  skipAuth() {
    if (this.devCredentials) {
      // Directly set the token and navigate without going through login flow
      localStorage.setItem('token', 'dev-bypass-token');
      this.snack.open('🚀 Authentication bypassed for development!', 'OK', { duration: 2000 });
      this.router.navigateByUrl('/', { replaceUrl: true });
    }
  }

  onSubmit() {
    if (this.form.invalid) return;
    this.loading.set(true);

    const { email, password } = this.form.getRawValue();
    this.auth.login(email, password).subscribe({
      next: (response) => {
        this.loading.set(false);
        // Save the token or flag in localStorage
        localStorage.setItem('token', response.token || 'dummy-token');

        // Different messages for dev vs regular login
        const isDevLogin = response.token === 'dev-bypass-token';
        const message = isDevLogin
          ? '🚀 Development bypass login successful!'
          : 'Logged in. Welcome back! 🔐';

        this.snack.open(message, 'OK', { duration: 2000 });
        // navigate to /
        this.router.navigateByUrl('/', { replaceUrl: true });
      },
      error: () => {
        this.loading.set(false);
        this.snack.open('Login failed. Check credentials.', 'OK', { duration: 2500 });
      }
    });
  }
}