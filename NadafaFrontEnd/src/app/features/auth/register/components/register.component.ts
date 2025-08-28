import { Component } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { Router, RouterModule } from '@angular/router';
import { AuthService } from '../../Services/auth.service';

@Component({
  selector: 'app-register',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, RouterModule],
  templateUrl: './register.component.html',
  styleUrls: ['./register.css']
})
export class Register {
  registerForm;

  constructor(
    private fb: FormBuilder,
    private authService: AuthService,
    private router: Router
  ) {
    this.registerForm = this.fb.group({
      name: ['', Validators.required],
      email: ['', [Validators.required, Validators.email]],
      password: ['', [Validators.required, Validators.minLength(6)]],
      confirmPassword: ['', [Validators.required, Validators.minLength(6)]],
      userType: ['Select...', Validators.required] // factory or house
    });
  }

  onSubmit() {
    const payload = this.registerForm.value as {
      name: string;
      email: string;
      password: string;
      confirmPassword: string;
      userType: 'factory' | 'house';
    };

    this.authService.register(payload).subscribe({
      next: (res: any) => {
        // save auth data in cookies
        document.cookie = `token=${res.token}; path=/;`;
        document.cookie = `role=${payload.userType}; path=/;`;

        // redirect based on role
        if (payload.userType === 'factory') {
          this.router.navigate(['/factory']);
        } else {
          this.router.navigate(['/house']);
        }
      },
      error: (err: any) => console.error('Error', err)
    });
  }
}
