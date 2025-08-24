import { Routes } from '@angular/router';
import { AuthLayoutComponent } from './features/auth/auth-layout.component';
import { AppComponent } from './app.component';
import { AuthGuard } from './core/auth/auth.guard';
export const appRoutes: Routes = [
  {
  path: '',
  component: AppComponent,
  canActivate: [AuthGuard], // we'll create this
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