import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, of } from 'rxjs';
import { delay, map } from 'rxjs/operators';

export interface RegisterPayload { email: string; password: string; name: string; }

@Injectable({ providedIn: 'root' })
export class AuthService {
  private http = inject(HttpClient);
  // TODO: set your API base
  private baseUrl = '/api/auth';

  login(email: string, password: string): Observable<{ token: string }>{
    // return this.http.post<{token:string}>(`${this.baseUrl}/login`, { email, password });
    return of({ token: 'fake-jwt' }).pipe(delay(800));
  }

  register(payload: RegisterPayload): Observable<{ userId: string }>{
    // return this.http.post<{userId:string}>(`${this.baseUrl}/register`, payload);
    return of({ userId: '123' }).pipe(delay(800));
  }

  requestPasswordReset(email: string): Observable<{ ok: boolean }>{
    // return this.http.post<{ok:boolean}>(`${this.baseUrl}/forgot-password`, { email });
    return of({ ok: true }).pipe(delay(800));
  }

  confirmEmail(code: string): Observable<{ ok: boolean }>{
    // return this.http.post<{ok:boolean}>(`${this.baseUrl}/confirm-email`, { code });
    return of({ ok: !!code && code.length > 3 }).pipe(delay(800));
  }

  resendEmailConfirmation(email: string): Observable<{ ok: boolean }>{
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
