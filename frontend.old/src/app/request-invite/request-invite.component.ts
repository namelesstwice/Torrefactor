import { Component, OnInit } from '@angular/core';
import {AuthService} from '../auth/auth.service';

@Component({
  selector: 'app-request-invite',
  templateUrl: './request-invite.component.html',
  styleUrls: ['./request-invite.component.css']
})
export class RequestInviteComponent implements OnInit {

  constructor(
    private authService: AuthService
  ) { }

  public email: string;
  public name: string;
  public emailSent: boolean;

  ngOnInit() {
  }

  async requestInvite() {
    await this.authService.requestInvite(this.email, this.name);
    this.emailSent = true;
  }
}
