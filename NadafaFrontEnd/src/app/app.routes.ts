import { Routes } from '@angular/router';
import { MainLayoutComponent } from './shared/layout/main-layout.component';
import { AuthLayoutComponent } from './features/auth/auth-layout.component';
import { AuthGuard } from './core/auth/auth.guard';

export const appRoutes: Routes = [
  {
    path: '',
    component: MainLayoutComponent,
    canActivate: [AuthGuard],
    children: [
      { path: '', redirectTo: 'dashboard', pathMatch: 'full' },
      { path: 'dashboard', loadComponent: () => import('./features/dashboard/dashboard.component').then(m => m.DashboardComponent) },
      { path: 'marketplace', loadComponent: () => import('./features/marketplace/marketplace.component').then(m => m.MarketplaceComponent) },
      { path: 'marketplace/:id', loadComponent: () => import('./features/marketplace/material-detail/material-detail.component').then(m => m.MaterialDetailComponent) },
      { path: 'profile', loadComponent: () => import('./features/dashboard/dashboard.component').then(m => m.DashboardComponent) }, // Placeholder
      { path: 'settings', loadComponent: () => import('./features/dashboard/dashboard.component').then(m => m.DashboardComponent) }, // Placeholder
      { path: 'security', loadComponent: () => import('./features/dashboard/dashboard.component').then(m => m.DashboardComponent) }, // Placeholder
    ]
  },
  {
    path: 'auth',
    component: AuthLayoutComponent,
    children: [
      { path: '', redirectTo: 'login', pathMatch: 'full' },
      { path: 'login', loadComponent: () => import('./features/auth/login/login.component').then(m => m.LoginComponent) },
      { path: 'register', loadComponent: () => import('./features/auth/register/register.component').then(m => m.RegisterComponent) },
      { path: 'forgot-password', loadComponent: () => import('./features/auth/forgot-password/forgot-password.component').then(m => m.ForgotPasswordComponent) },
      { path: 'verify-email', loadComponent: () => import('./features/auth/verify-email/verify-email.component').then(m => m.VerifyEmailComponent) },
    ]
  },
  { path: '**', redirectTo: 'auth/login' }
];