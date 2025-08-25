import { Routes } from '@angular/router';
// import { AuthLayoutComponent } from './features/auth/auth-layout.component';
import { AppComponent } from './app.component';
import { AuthGuard } from './core/auth/auth.guard';
export const appRoutes: Routes = [
  {
  path: '',
  component: AppComponent,
  canActivate: [AuthGuard],
},
{
  path: 'auth',
  component: AppComponent,
  children: [
    { path: '', redirectTo: 'login', pathMatch: 'full' },
    { path: 'login', loadComponent: () => import('./features/auth/login/login.component').then(m => m.Login) },
    { path: 'register', loadComponent: () => import('./features/auth/register/register.component').then(m => m.Register) },
    { path: 'forgot-password', loadComponent: () => import('./features/auth/forget-password/forget-password.component').then(m => m.ForgetPassword) },
    { path: 'verify-email', loadComponent: () => import('./features/auth/verify-email/verify-email.component').then(m => m.VerifyEmail) },
    { path: '', loadComponent: () => import('./app.component').then(m => m.AppComponent) },
  ]
},
{ path: '**', redirectTo: 'auth/login' }

];