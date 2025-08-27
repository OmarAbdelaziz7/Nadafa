import { Component } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { RouterLink } from '@angular/router';

@Component({
  selector: 'app-forget-password',
  imports: [RouterLink],
  templateUrl: './forget-password.html',
  styleUrls: ['./forget-password.css',
    '../../../../../node_modules/bootstrap/dist/css/bootstrap.min.css'
  ]
})
export class ForgetPassword {
forgotForm: FormGroup;

  constructor(private fb: FormBuilder) {
    this.forgotForm = this.fb.group({
      email: ['', [Validators.required, Validators.email]]
    });
  }

  onSubmit() {
    if (this.forgotForm.valid) {
      console.log('Send reset link to:', this.forgotForm.value.email);
      // Call backend API here
    }
  }
}
