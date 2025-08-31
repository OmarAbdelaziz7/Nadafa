import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, tap } from 'rxjs';
import { LoginRequest } from '../login/DTO/login.dto';
import { RegisterRequest } from '../register/DTO/register.dto';
import { CookieService } from 'ngx-cookie-service';
import { ForgotPasswordRequest } from '../forget-password/DTO/forget-password.dto';
import { VerifyEmailDto } from '../verify-email/DTO/verify-email.dto';

@Injectable({ providedIn: 'root' })
export class AuthService {
  private baseUrl = 'https://localhost:4200/auth';
  private tokenKey = 'token';
  private roleKey = 'role';

  constructor(private http: HttpClient, private cookies: CookieService) {}

  login(payload: LoginRequest): Observable<any> {
    return this.http.post(`${this.baseUrl}/login`, payload).pipe(
      tap((res: any) => {
        this.cookies.set(this.tokenKey, res.token, { path: '/', secure: true });
        this.cookies.set(this.roleKey, res.role, { path: '/', secure: true });
      })
    );
  }

  register(payload: RegisterRequest): Observable<any> {
    return this.http.post(`${this.baseUrl}/register`, payload).pipe(
      tap((res: any) => {
        this.cookies.set(this.tokenKey, res.token, { path: '/', secure: true });
        this.cookies.set(this.roleKey, res.role, { path: '/', secure: true });
      })
    );
  }

  forgotPassword(payload: ForgotPasswordRequest): Observable<any> {
    return this.http.post(`${this.baseUrl}/forgot-password`, payload);
  }

  verifyEmail(payload: VerifyEmailDto): Observable<any> {
    return this.http.post(`${this.baseUrl}/verify-email`, payload);
  }


  logout(): void {
    this.cookies.delete(this.tokenKey, '/');
    this.cookies.delete(this.roleKey, '/');
  }

  isLoggedIn(): boolean {
    return this.cookies.check(this.tokenKey);
  }

  getRole(): string | null {
    return this.cookies.get(this.roleKey) || null;
  }
}
