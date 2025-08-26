import { Component } from '@angular/core';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { MatCardModule } from '@angular/material/card';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { RouterLink } from '@angular/router';

@Component({
    selector: 'app-forgot-password',
    standalone: true,
    imports: [
        ReactiveFormsModule,
        MatCardModule,
        MatFormFieldModule,
        MatInputModule,
        MatButtonModule,
        RouterLink
    ],
    template: `
    <mat-card style="padding: 1rem;">
      <mat-card-header>
        <mat-card-title>Reset Password</mat-card-title>
        <mat-card-subtitle>Enter your email to receive reset instructions</mat-card-subtitle>
      </mat-card-header>
      
      <mat-card-content>
        <form [formGroup]="forgotForm" (ngSubmit)="onSubmit()">
          <mat-form-field appearance="outline" class="w-full">
            <mat-label>Email</mat-label>
            <input matInput type="email" formControlName="email" required />
          </mat-form-field>
        </form>
      </mat-card-content>
      
      <mat-card-actions>
        <button mat-flat-button color="primary" (click)="onSubmit()" [disabled]="forgotForm.invalid">
          Send Reset Instructions
        </button>
        <a mat-button routerLink="/auth/login">Back to Login</a>
      </mat-card-actions>
    </mat-card>
  `,
    styles: [`
    .w-full { width: 100%; }
  `]
})
export class ForgotPasswordComponent {
    forgotForm: FormGroup;

    constructor(private fb: FormBuilder) {
        this.forgotForm = this.fb.group({
            email: ['', [Validators.required, Validators.email]]
        });
    }

    onSubmit() {
        if (this.forgotForm.valid) {
            console.log('Password reset requested for:', this.forgotForm.value.email);
            // TODO: Implement password reset logic
        }
    }
}
