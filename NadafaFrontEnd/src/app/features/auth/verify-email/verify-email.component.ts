import { Component } from '@angular/core';
import { MatCardModule } from '@angular/material/card';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { RouterLink } from '@angular/router';

@Component({
  selector: 'app-verify-email',
  standalone: true,
  imports: [
    MatCardModule,
    MatButtonModule,
    MatIconModule,
    RouterLink
  ],
  template: `
    <mat-card style="padding: 1rem; text-align: center;">
      <mat-card-header>
        <mat-card-title>Verify Your Email</mat-card-title>
        <mat-card-subtitle>Check your inbox for verification instructions</mat-card-subtitle>
      </mat-card-header>
      
      <mat-card-content>
        <mat-icon style="font-size: 3rem; color: #4CAF50; margin: 1rem 0;">email</mat-icon>
        <p>We've sent a verification link to your email address. Please check your inbox and click the link to verify your account.</p>
      </mat-card-content>
      
      <mat-card-actions>
        <button mat-flat-button color="primary" (click)="resendVerification()">
          Resend Verification Email
        </button>
        <a mat-button routerLink="/auth/login">Back to Login</a>
      </mat-card-actions>
    </mat-card>
  `,
  styleUrl: './verify-email.css'
})
export class VerifyEmailComponent {
  resendVerification() {
    console.log('Resend verification link');
    // TODO: Implement resend verification logic
  }
}