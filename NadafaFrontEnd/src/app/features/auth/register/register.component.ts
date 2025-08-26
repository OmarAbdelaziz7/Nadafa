import { Component } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';

@Component({
  selector: 'app-register',
  imports: [],
  templateUrl: './register.html',
  styleUrls: ['./register.css',
    '../../../../../node_modules/bootstrap/dist/css/bootstrap.min.css'
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
