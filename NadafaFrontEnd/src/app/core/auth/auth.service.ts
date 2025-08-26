import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, of, throwError } from 'rxjs';
import { delay, map } from 'rxjs/operators';

export interface RegisterPayload { email: string; password: string; name: string; }

@Injectable({ providedIn: 'root' })
export class AuthService {
  private http = inject(HttpClient);
  // TODO: set your API base
  private baseUrl = '/api/auth';

  // Development bypass credentials - REMOVE IN PRODUCTION!
  private readonly DEV_CREDENTIALS = {
    email: 'dev@nadafa.local',
    password: 'dev123',
    displayName: 'Developer'
  };

  login(email: string, password: string): Observable<{ token: string }> {
    // Check for development bypass credentials
    if (this.isDevelopmentMode() && this.isDevCredentials(email, password)) {
      console.log('🚀 Development bypass login successful!');
      return of({ token: 'dev-bypass-token' }).pipe(delay(300));
    }

    // For non-dev credentials, simulate API call (replace with real API in production)
    if (email && password) {
      return of({ token: 'fake-jwt' }).pipe(delay(800));
    }

    // Simulate login failure for empty credentials
    return throwError(() => new Error('Invalid credentials'));
  }

  private isDevelopmentMode(): boolean {
    // Check if we're in development mode
    return !!(window as any)['ng'] || location.hostname === 'localhost' || location.hostname === '127.0.0.1';
  }

  private isDevCredentials(email: string, password: string): boolean {
    return email === this.DEV_CREDENTIALS.email && password === this.DEV_CREDENTIALS.password;
  }

  getDevCredentials() {
    return this.isDevelopmentMode() ? this.DEV_CREDENTIALS : null;
  }

  register(payload: RegisterPayload): Observable<{ userId: string }> {
    // return this.http.post<{userId:string}>(`${this.baseUrl}/register`, payload);
    return of({ userId: '123' }).pipe(delay(800));
  }

  requestPasswordReset(email: string): Observable<{ ok: boolean }> {
    // return this.http.post<{ok:boolean}>(`${this.baseUrl}/forgot-password`, { email });
    return of({ ok: true }).pipe(delay(800));
  }

  confirmEmail(code: string): Observable<{ ok: boolean }> {
    // return this.http.post<{ok:boolean}>(`${this.baseUrl}/confirm-email`, { code });
    return of({ ok: !!code && code.length > 3 }).pipe(delay(800));
  }

  resendEmailConfirmation(email: string): Observable<{ ok: boolean }> {
    // return this.http.post<{ok:boolean}>(`${this.baseUrl}/resend-confirmation`, { email });
    return of({ ok: true }).pipe(delay(800));
  }

  isLoggedIn(): boolean {
    return !!localStorage.getItem('token');
  }


  logout() {
    localStorage.removeItem('token');
  }
}
