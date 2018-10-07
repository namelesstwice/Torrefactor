import { Injectable } from '@angular/core';
import { ActivatedRouteSnapshot, Resolve, Router, RouterStateSnapshot } from '@angular/router';
import { HttpClient } from '@angular/common/http';
import { Invite } from './invite';

@Injectable({
  providedIn: 'root',
})
export class AuthService implements Resolve<Invite[]> {
  constructor(
    private router: Router,
    private http: HttpClient
  ) {}

  private isLoggedIn: boolean = null;
  redirectUrl: string;

  async signIn(email: string, password: string) {
    this.isLoggedIn = true;
    await (this.http.post('/api/auth/sign-in', { email: email, password: password}).toPromise());
    await this.router.navigate([this.redirectUrl || '/']);
  }

  async isAuthenticated() {
    if (this.isLoggedIn !== null) {
      return this.isLoggedIn;
    }

    const authResult = (await (this.http.get('/api/auth/user')).toPromise()) as any;
    this.isLoggedIn = authResult.name !== null;
    return this.isLoggedIn;
  }

  async requestInvite(email: string, name: string) {
    await (this.http.post('api/auth/invite', { email: email, name: name })).toPromise();
  }

  async getPendingInviteRequests() {
    const result = await (this.http.get('api/auth/invite')).toPromise();
    return result as Invite[];
  }

  async signOut() {
    await this.http.post('/api/auth/sign-out', {}).toPromise();
    this.isLoggedIn = false;
    await this.router.navigate(['/sign-in']);
  }

  async register(token: string, password: string) {
    await this.http.post('/api/auth/register', { token: token, password: password}).toPromise();
    await this.router.navigate(['/']);
  }

  async acceptOrDecline(invite: Invite, isApproved: boolean) {
    await this.http.post(`/api/auth/invite/state?id=${invite.id}&isApproved=${isApproved}`, {}).toPromise();
  }

  async resolve(route: ActivatedRouteSnapshot, state: RouterStateSnapshot) {
    return await this.getPendingInviteRequests();
  }
}
