import { Component, inject } from '@angular/core';
import { ReactiveFormsModule, FormBuilder, Validators } from '@angular/forms';
import { RouterLink } from '@angular/router';
import { MatCardModule } from '@angular/material/card';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { MatSnackBar, MatSnackBarModule } from '@angular/material/snack-bar';
import { AuthService } from '../../../core/auth/auth.service';

@Component({
  standalone: true,
  selector: 'app-verify-email',
  imports: [
    ReactiveFormsModule, RouterLink,
    MatCardModule, MatFormFieldModule, MatInputModule, MatButtonModule, MatSnackBarModule,
  ],
  styles: [` .card { padding: 1rem; } .row{display:flex;gap:.5rem;align-items:center} .mt{margin-top:.5rem} `],
  template: `
    <mat-card class="card">
      <form [formGroup]="codeForm" (ngSubmit)="onConfirm()">
        <mat-form-field appearance="outline" class="w-full">
          <mat-label>Confirmation Code</mat-label>
          <input matInput type="text" formControlName="code" required />
        </mat-form-field>
        <button mat-flat-button color="primary" type="submit" [disabled]="codeForm.invalid || loading">{{ loading ? 'Verifying…' : 'Verify email' }}</button>
      </form>

      <div class="mt">
        <div class="row">
          <form [formGroup]="resendForm" (ngSubmit)="onResend()" class="row">
            <mat-form-field appearance="outline" class="w-full">
              <mat-label>Email</mat-label>
              <input matInput type="email" formControlName="email" placeholder="you@example.com" />
            </mat-form-field>
            <button mat-stroked-button type="submit" [disabled]="resendForm.invalid || sending">{{ sending ? 'Sending…' : 'Resend code' }}</button>
          </form>
        </div>
      </div>

      <div class="mt"><a routerLink="/auth/login">Back to login</a></div>
    </mat-card>
  `,
})
export class VerifyEmailComponent {
  private fb = inject(FormBuilder);
  private snack = inject(MatSnackBar);
  private auth = inject(AuthService);

  loading = false;
  sending = false;

  codeForm = this.fb.nonNullable.group({ code: ['', [Validators.required, Validators.minLength(4)]] });
  resendForm = this.fb.nonNullable.group({ email: ['', [Validators.required, Validators.email]] });

  onConfirm() {
    if (this.codeForm.invalid) return;
    this.loading = true;
    this.auth.confirmEmail(this.codeForm.value.code!).subscribe({
      next: (res) => {
        this.loading = false;
        this.snack.open(res.ok ? 'Email confirmed. 🎉' : 'Invalid code.', 'OK', { duration: 2500 });
      },
      error: () => {
        this.loading = false;
        this.snack.open('Verification failed.', 'OK', { duration: 2500 });
      }
    });
  }

  onResend() {
    if (this.resendForm.invalid) return;
    this.sending = true;
    this.auth.resendEmailConfirmation(this.resendForm.value.email!).subscribe({
      next: () => {
        this.sending = false;
        this.snack.open('Confirmation email sent. 📮', 'OK', { duration: 2500 });
      },
      error: () => {
        this.sending = false;
        this.snack.open('Could not resend.', 'OK', { duration: 2500 });
      }
    });
  }
}