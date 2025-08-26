import { Component } from '@angular/core';
import { RouterOutlet } from '@angular/router';

@Component({
  selector: 'app-auth-layout',
  standalone: true,
  imports: [RouterOutlet],
  styles: [`
    :host { 
      display: grid; 
      min-height: 100dvh; 
      place-items: center; 
      background: #0f172a; 
    }
    .shell { 
      width: min(96vw, 420px); 
    }
    .logo { 
      display: flex; 
      align-items: center; 
      gap: .5rem; 
      color: #e2e8f0; 
      justify-content: center; 
      margin-bottom: .5rem; 
    }
    .title { 
      text-align: center; 
      color: #f8fafc; 
      font-weight: 700; 
      font-size: 1.25rem; 
      margin-bottom: 1rem; 
    }
    .muted { 
      text-align: center; 
      color: #94a3b8; 
      font-size: .9rem; 
      margin-bottom: 1rem; 
    }
  `],
  template: `
    <div class="shell">
      <div class="logo">
        <span style="font-weight:800;">⚡ Nadafa</span>
      </div>
      <div class="title">Welcome</div>
      <div class="muted">Sign in, create an account, reset password, or confirm email.</div>
      <router-outlet></router-outlet>
    </div>
  `
})
export class AuthLayoutComponent { }