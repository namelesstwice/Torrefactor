import { Component, OnInit } from '@angular/core';
import {AuthService} from '../auth/auth.service';

@Component({
  selector: 'app-sign-in',
  templateUrl: './sign-in.component.html',
  styleUrls: ['./sign-in.component.css']
})
export class SignInComponent implements OnInit {

  constructor(
    private authService: AuthService,
  ) { }

  public email: string;
  public password: string;

  ngOnInit() {
  }

  async signIn() {
    await this.authService.signIn(this.email, this.password);
  }
}
