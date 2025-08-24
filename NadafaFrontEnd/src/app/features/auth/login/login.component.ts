import { Component } from '@angular/core';

@Component({
  selector: 'app-login',
  imports: [],
  templateUrl: './login.html',
  styleUrl: './login.css'
})
export class Login {

}
import { inject, signal } from '@angular/core';
import { ReactiveFormsModule, FormBuilder, Validators } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { MatCardModule } from '@angular/material/card';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatSnackBar, MatSnackBarModule } from '@angular/material/snack-bar';
import { AuthService } from '../../../core/auth/auth.service';
import { NgIf } from '@angular/common';

@Component({
  standalone: true,
  selector: 'app-login',
  imports: [
    ReactiveFormsModule, RouterLink, NgIf,
    MatCardModule, MatFormFieldModule, MatInputModule,
    MatButtonModule, MatIconModule, MatSnackBarModule,
  ],
  styles: [`
    .card { padding: 1rem; }
    .actions { display:flex; gap:.75rem; align-items:center; justify-content:space-between; margin-top:.5rem; }
    .links { display:flex; gap:.75rem; justify-content:center; margin-top:.75rem; }
  `],
  template: `
    <mat-card class="card">
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
  toggleHide() { this.hide.set(!this.hide()); }

  form = this.fb.nonNullable.group({
    email: ['', [Validators.required, Validators.email]],
    password: ['', [Validators.required, Validators.minLength(6)]],
  });

onSubmit() {
  if (this.form.invalid) return;
  this.loading.set(true);

  const { email, password } = this.form.getRawValue();
  this.auth.login(email, password).subscribe({
    next: (response) => {
      this.loading.set(false);
      // Save the token or flag in localStorage
      localStorage.setItem('token', response.token || 'dummy-token'); 
      // show snackbar
      this.snack.open('Logged in. Welcome back! 🔐', 'OK', { duration: 2000 });
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