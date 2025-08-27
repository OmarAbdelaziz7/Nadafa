import { Component } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { RouterLink } from '@angular/router';

@Component({
  selector: 'app-register',
  imports: [RouterLink],
  templateUrl: './register.html',
  styleUrls: ['./register.css',
  ]
})
export class Register {
   rgisterForm: FormGroup;

  constructor(private fb: FormBuilder) {
    this.rgisterForm = this.fb.group({
      email: ['', [Validators.required, Validators.email]],
      password: ['', [Validators.required, Validators.minLength(6)]]
    });
  }

  onSubmit() {
    if (this.rgisterForm.valid) {
      console.log(this.rgisterForm.value);
    }
  }

}
