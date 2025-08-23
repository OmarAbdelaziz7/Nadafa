import { Component, inject } from '@angular/core';
import { ReactiveFormsModule, FormBuilder, Validators } from '@angular/forms';
import { RouterLink } from '@angular/router';
import { MatCardModule } from '@angular/material/card';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { MatSnackBar, MatSnackBarModule } from '@angular/material/snack-bar';
import { passwordMatchValidator } from '../../../shared/validators/password-match.validator';
import { AuthService } from '../../../core/auth.service';
import { NgIf } from '@angular/common';


@Component({
standalone: true,
selector: 'app-register',
templateUrl: './register.component.html',
styleUrls: ['./register.component.css'],
imports: [ReactiveFormsModule, RouterLink, NgIf, MatCardModule, MatFormFieldModule, MatInputModule, MatButtonModule, MatSnackBarModule]
})
export class RegisterComponent {
fb = inject(FormBuilder);
snack = inject(MatSnackBar);
auth = inject(AuthService);
loading = false;
form = this.fb.nonNullable.group({
name: ['', [Validators.required, Validators.minLength(2)]],
email: ['', [Validators.required, Validators.email]],
password: ['', [Validators.required, Validators.minLength(6)]],
confirmPassword: ['', [Validators.required]]
}, { validators: passwordMatchValidator() });
onSubmit() { /* same logic as before */ }
}