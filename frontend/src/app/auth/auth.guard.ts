import { Injectable } from '@angular/core';
import { CanActivate, ActivatedRouteSnapshot, RouterStateSnapshot, Router } from '@angular/router';
import { AuthService } from './auth.service';
import {Observable} from 'rxjs';
import {ancestorWhere} from 'tslint';

@Injectable({
  providedIn: 'root'
})
export class AuthGuard implements CanActivate {
  constructor(
    private authService: AuthService,
    private router: Router) {}

  canActivate(next: ActivatedRouteSnapshot, state: RouterStateSnapshot): Promise<boolean> {
    const url: string = state.url;
    return this.checkLogin(url);
  }

  async checkLogin(url: string): Promise<boolean> {
    if (await this.authService.isAuthenticated()) {
      return true;
    }

    // Store the attempted URL for redirecting
    this.authService.redirectUrl = url;

    // Navigate to the signIn page with extras
    this.router.navigate(['/sign-in']);
    return false;
  }
}
