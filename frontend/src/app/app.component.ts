import { Component } from '@angular/core';
import { AuthService } from './_services/auth-service.service';
import { Router } from '@angular/router';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.less']
})
export class AppComponent {
  
  constructor(
    private auth: AuthService,
    private router: Router) {}

  public get isAdmin() {
    return this.auth.isAuthenticated;
  }  

  public get isAuthenticated() {
    return this.auth.isAuthenticated;
  }  

  public logout() {
    this.auth.logout();
    this.router.navigate(['/sign-in']);
  }
}
