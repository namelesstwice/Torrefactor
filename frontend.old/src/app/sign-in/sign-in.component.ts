import { Component, OnInit } from '@angular/core';
import {AuthService} from '../auth/auth.service';
import {Router} from '@angular/router';

@Component({
  selector: 'app-sign-in',
  templateUrl: './sign-in.component.html',
  styleUrls: ['./sign-in.component.css']
})
export class SignInComponent implements OnInit {

  constructor(
    private authService: AuthService,
    private router: Router,
  ) { }

  public isAuthFailed: boolean;
  public email: string;
  public password: string;

  ngOnInit() {
  }

  async signIn() {
    this.isAuthFailed = false;
    if (! await this.authService.signIn(this.email, this.password)) {
      this.isAuthFailed = true;
    }
  }
}
