import { Component, OnInit } from '@angular/core';
import { AuthService, AuthError } from '../auth-service.service'
import { Router } from '@angular/router';

@Component({
  selector: 'app-registration-form',
  templateUrl: './registration-form.component.html',
  styleUrls: ['./registration-form.component.less']
})
export class RegistrationFormComponent implements OnInit {

  constructor(
    private auth: AuthService,
    private router: Router) { }

  ngOnInit(): void {
  }

  async onSubmit() {
    try {
      this.isSending = true;
      this.serverError = '';
      await this.auth.register(this.name, this.email, this.password).toPromise();
      this.router.navigate(['/registration-completed']);
    } catch (error) {
      if (error instanceof AuthError) {
        this.serverError = error.errorText;
      } else {
        this.serverError = 'Unexpected error, please try again later.'
      }
    } finally {
      this.isSending = false;
    }
  }

  public name: string = '';
  public email: string = '';
  public password: string = '';
  public serverError: string = '';
  public isSending = false;
}
