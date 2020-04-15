import { Component, OnInit } from '@angular/core';
import { AuthService } from '../auth/auth.service';
import {UserService} from '../auth/user.service';

@Component({
  selector: 'app-top-menu',
  templateUrl: './top-menu.component.html',
  styleUrls: ['./top-menu.component.css']
})
export class TopMenuComponent implements OnInit {

  constructor(
    private authService: AuthService,
    private userService: UserService,
  ) { }

  public isAuthenticated = false;
  public isAdmin = false;

  ngOnInit() {
    this.userService.userObservable.subscribe((user) => {
      this.isAuthenticated = user != null;
      this.isAdmin = this.isAuthenticated && user.isAdmin;
    });
  }

  async signOut() {
    await this.authService.signOut();
  }

}
