import { Injectable } from '@angular/core';
import { of } from 'rxjs'; // Replace with HttpClient later for real API calls

@Injectable({ providedIn: 'root' })
export class AuthService {
  login(credentials: { email: string; password: string }) {
    // Example: return this.http.post('/api/login', credentials);
    return of({ success: true });
  }

  register(data: { name: string; email: string; password: string }) {
    return of({ success: true });
  }

  forgotPassword(email: string) {
    return of({ success: true });
  }

  verifyEmail(token: string) {
    return of({ success: true });
  }
}
