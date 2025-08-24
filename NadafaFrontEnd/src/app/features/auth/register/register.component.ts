import { Component, inject } from '@angular/core';
import { ReactiveFormsModule, FormBuilder, Validators } from '@angular/forms';
import { RouterLink } from '@angular/router';
import { NgIf } from '@angular/common';
import { MatCardModule } from '@angular/material/card';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { MatSnackBar, MatSnackBarModule } from '@angular/material/snack-bar';
import { passwordMatchValidator } from '../shared/password-match.validator';
import { AuthService } from '../core/auth/auth.service';

@Component({
  standalone: true,
  selector: 'app-register',
  imports: [
    ReactiveFormsModule, RouterLink, NgIf,
    MatCardModule, MatFormFieldModule, MatInputModule, MatButtonModule, MatSnackBarModule,
  ],
  styles: [`
    .card { padding: 1rem; }
    .actions { display:flex; gap:.75rem; align-items:center; justify-content:space-between; margin-top:.5rem; }
    .links { display:flex; gap:.5rem; justify-content:center; margin-top:.75rem; }
  `],
  template: `
    <mat-card class="card">
      <form [formGroup]="form" (ngSubmit)="onSubmit()">
        <mat-form-field appearance="outline" class="w-full">
          <mat-label>Name</mat-label>
          <input matInput type="text" formControlName="name" required />
        </mat-form-field>

        <mat-form-field appearance="outline" class="w-full">
          <mat-label>Email</mat-label>
          <input matInput type="email" formControlName="email" required />
        </mat-form-field>

        <mat-form-field appearance="outline" class="w-full">
          <mat-label>Password</mat-label>
          <input matInput type="password" formControlName="password" required />
        </mat-form-field>

        <mat-form-field appearance="outline" class="w-full">
          <mat-label>Confirm Password</mat-label>
          <input matInput type="password" formControlName="confirmPassword" required />
        </mat-form-field>

        <div class="actions">
          <button mat-flat-button color="primary" type="submit" [disabled]="form.invalid || loading">{{ loading ? 'Creating…' : 'Create account' }}</button>
          <a routerLink="/auth/login">Have an account? Sign in</a>
        </div>

        <div *ngIf="form.errors?.['passwordMismatch']" class="links" style="color:#ef4444;">Passwords do not match.</div>
      </form>
    </mat-card>
  `,
})
export class RegisterComponent {
  private fb = inject(FormBuilder);
  private snack = inject(MatSnackBar);
  private auth = inject(AuthService);

  loading = false;

  form = this.fb.nonNullable.group({
    name: ['', [Validators.required, Validators.minLength(2)]],
    email: ['', [Validators.required, Validators.email]],
    password: ['', [Validators.required, Validators.minLength(6)]],
    confirmPassword: ['', [Validators.required]],
  }, { validators: passwordMatchValidator() });

  onSubmit() {
    if (this.form.invalid) return;
    this.loading = true;
    const { name, email, password } = this.form.getRawValue();
    this.auth.register({ name, email, password }).subscribe({
      next: () => {
        this.loading = false;
        this.snack.open('Account created. Check your email to confirm. 📧', 'OK', { duration: 2500 });
      },
      error: () => {
        this.loading = false;
        this.snack.open('Registration failed. Try again.', 'OK', { duration: 2500 });
      }
    });
  }
}