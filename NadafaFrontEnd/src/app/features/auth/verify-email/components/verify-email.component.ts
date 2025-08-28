import { Component } from '@angular/core';
import { RouterLink } from '@angular/router';

@Component({
  selector: 'app-verify-email',
  imports: [RouterLink],
  templateUrl: './verify-email.component.html',
  styleUrl: './verify-email.css'
})
export class VerifyEmail {
  resendVerification() {
    console.log('Resend verification link');
}

}