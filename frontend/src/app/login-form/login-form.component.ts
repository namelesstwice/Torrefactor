import { Component, OnInit } from '@angular/core';
import { AuthService, AuthError } from '../_services/auth-service.service';
import { Router } from '@angular/router';

@Component({
  selector: 'app-login-form',
  templateUrl: './login-form.component.html',
  styleUrls: ['./login-form.component.less']
})
export class LoginFormComponent implements OnInit {

  constructor(
    private auth: AuthService,
    private router: Router) { }

  ngOnInit(): void {
  }

  async login() {
    try {
      this.serverError = '';
      await this.auth.login(this.email, this.password).toPromise();
      this.router.navigate(['/coffee-kinds'])
    } catch (error) {
      if (error instanceof AuthError) {
        this.serverError = error.errorText;
      }
    }
  }

  public email = '';
  public password = '';
  public serverError = '';
}
