import { Component } from '@angular/core';
import { FormBuilder, FormsModule, ReactiveFormsModule, Validators } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { Router, RouterLink, RouterModule } from '@angular/router';
import { AuthService } from '../../Services/auth.service';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, RouterModule, FormsModule, RouterLink],
  templateUrl: './login.component.html',
  styleUrls: ['./login.css']
})
export class Login {
  loginForm;

  constructor(
    private fb: FormBuilder,
    private authService: AuthService,
    private router: Router
  ) {
    this.loginForm = this.fb.group({
      email: ['', [Validators.required, Validators.email]],
      password: ['', Validators.required],
      userType: ['Select...', Validators.required]
    });
  }

  onSubmit() {
    if (this.loginForm.valid) {
      const payload = this.loginForm.value as { 
        email: string; 
        password: string; 
        userType: 'factory' | 'house'; 
      };

      this.authService.login(payload).subscribe({
        next: (res: any) => {
          // store token + role in cookies
            document.cookie = `token=${res.token}; path=/;`;
            document.cookie = `role=${payload.userType}; path=/;`;

          // redirect based on role
          if (payload.userType === 'factory') {
            this.router.navigate(['/factory']);
          } else {
            this.router.navigate(['house-products']);
          }
        },
        error: (err: any) => console.error('Error', err)
      });
    }
  }
}
