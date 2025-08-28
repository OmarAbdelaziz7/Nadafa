import { Injectable } from '@angular/core';
import { AuthService } from '../Services/auth.service';
import { LoginDto } from '../login/DTO/login.dto';
import { RegisterDto } from '../register/DTO/register.dto';
import { ForgotPasswordDto } from '../forget-password/DTO/forget-password.dto';
import { VerifyEmailDto } from '../verify-email/DTO/verify-email.dto';
import { Observable, tap } from 'rxjs';

@Injectable({ providedIn: 'root' })
export class AuthRepository {
  private tokenKey = 'auth_token';

  constructor(private authService: AuthService) {}

  login(payload: LoginDto): Observable<any> {
    return this.authService.login(payload).pipe(
      tap((res: any) => {
        localStorage.setItem(this.tokenKey, res.token);
      })
    );
  }

  register(payload: RegisterDto): Observable<any> {
    return this.authService.register(payload);
  }

  forgotPassword(payload: ForgotPasswordDto): Observable<any> {
    return this.authService.forgotPassword(payload);
  }

  verifyEmail(payload: VerifyEmailDto): Observable<any> {
    return this.authService.verifyEmail(payload);
  }

  isLoggedIn(): boolean {
    return !!localStorage.getItem(this.tokenKey);
  }

  logout(): void {
    localStorage.removeItem(this.tokenKey);
  }
}
