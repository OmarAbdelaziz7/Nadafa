import { Routes } from '@angular/router';
// import { AuthLayoutComponent } from './features/auth/auth-layout.component';
import { AppComponent } from './app.component';
import { AuthGuard } from './core/auth//guards/auth.guard';
import { RoleGuard } from './core/auth/guards/role.guard';
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
    { path: 'login', loadComponent: () => import('./features/auth/login/Components/login.component').then(m => m.Login) },
    { path: 'register', loadComponent: () => import('./features/auth/register/components/register.component').then(m => m.Register) },
    { path: 'forgot-password', loadComponent: () => import('./features/auth/forget-password/components/forget-password.component').then(m => m.ForgetPassword) },
    { path: 'verify-email', loadComponent: () => import('./features/auth/verify-email/components/verify-email.component').then(m => m.VerifyEmail) },
    { path: '', loadComponent: () => import('./app.component').then(m => m.AppComponent) },
      {
    path: 'factory',
    // loadComponent: () => import('./features/factory/factory.component').then(m => m.FactoryComponent),
    canActivate: [RoleGuard],
    data: { role: 'Factory' }
  },
  {
    path: 'house',
    // loadComponent: () => import('./features/house/house.component').then(m => m.HouseComponent),
    canActivate: [RoleGuard],
    data: { role: 'House' }
  }
  ]
},
{ path: '**', redirectTo: 'auth/login' }

];