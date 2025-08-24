import { Routes } from '@angular/router';
import { AuthLayoutComponent } from './auth/auth-layout.component';

export const appRoutes: Routes = [
  { path: '', redirectTo: 'auth/login', pathMatch: 'full' },
  {
    path: 'auth',
    component: AuthLayoutComponent,
    children: [
      { path: '', redirectTo: 'login', pathMatch: 'full' },
      { path: 'login', loadComponent: () => import('./auth/login.component').then(m => m.LoginComponent) },
      { path: 'register', loadComponent: () => import('./auth/register.component').then(m => m.RegisterComponent) },
      { path: 'forgot-password', loadComponent: () => import('./auth/forgot-password.component').then(m => m.ForgotPasswordComponent) },
      { path: 'verify-email', loadComponent: () => import('./auth/verify-email.component').then(m => m.VerifyEmailComponent) },
    ],
  },
  { path: '**', redirectTo: 'auth/login' },
];