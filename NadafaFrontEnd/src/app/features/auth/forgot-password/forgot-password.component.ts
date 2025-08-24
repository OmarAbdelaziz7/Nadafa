import { Component, inject } from '@angular/core';
import { ReactiveFormsModule, FormBuilder, Validators } from '@angular/forms';
import { RouterLink } from '@angular/router';
import { MatCardModule } from '@angular/material/card';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { MatSnackBar, MatSnackBarModule } from '@angular/material/snack-bar';
import { AuthService } from '../core/auth/auth.service';

@Component({
  standalone: true,
  selector: 'app-forgot-password',
  imports: [
    ReactiveFormsModule, RouterLink,
    MatCardModule, MatFormFieldModule, MatInputModule, MatButtonModule, MatSnackBarModule,
  ],
  styles: [` .card { padding: 1rem; } .links{display:flex;justify-content:center;margin-top:.75rem;} `],
  template: `
    <mat-card class="card">
      <form [formGroup]="form" (ngSubmit)="onSubmit()">
        <mat-form-field appearance="outline" class="w-full">
          <mat-label>Email</mat-label>
          <input matInput type="email" formControlName="email" required />
        </mat-form-field>
        <button mat-flat-button color="primary" type="submit" [disabled]="form.invalid || loading">{{ loading ? 'Sending…' : 'Send reset link' }}</button>
      </form>
      <div class="links"><a routerLink="/auth/login">Back to login</a></div>
    </mat-card>
  `,
})
export class ForgotPasswordComponent {
  private fb = inject(FormBuilder);
  private snack = inject(MatSnackBar);
  private auth = inject(AuthService);

  loading = false;
  form = this.fb.nonNullable.group({ email: ['', [Validators.required, Validators.email]] });

  onSubmit() {
    if (this.form.invalid) return;
    this.loading = true;
    this.auth.requestPasswordReset(this.form.value.email!).subscribe({
      next: () => {
        this.loading = false;
        this.snack.open('Reset link sent. Check your inbox. ✉️', 'OK', { duration: 2500 });
      },
      error: () => {
        this.loading = false;
        this.snack.open('Could not send reset link.', 'OK', { duration: 2500 });
      }
    });
  }
}