import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import {AuthService} from '../auth/auth.service';

@Component({
  selector: 'app-complete-registration',
  templateUrl: './complete-registration.component.html',
  styleUrls: ['./complete-registration.component.css']
})
export class CompleteRegistrationComponent implements OnInit {

  constructor(
    private route: ActivatedRoute,
    private authService: AuthService
  ) { }

  private token: string;
  public password: string;

  ngOnInit() {
    this.token = this.route.snapshot.queryParamMap.get('token');
  }

  async register() {
    await this.authService.register(this.token, this.password);
  }
}
