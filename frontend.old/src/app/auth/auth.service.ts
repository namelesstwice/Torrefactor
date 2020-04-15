import { Injectable } from '@angular/core';
import { ActivatedRouteSnapshot, Resolve, Router, RouterStateSnapshot } from '@angular/router';
import { HttpClient } from '@angular/common/http';
import { Invite } from './invite';
import { UserService } from './user.service';

@Injectable({
  providedIn: 'root',
})
export class AuthService implements Resolve<Invite[]> {
  constructor(
    private router: Router,
    private http: HttpClient,
    private userService: UserService,
  ) {}

  redirectUrl: string;

  async signIn(email: string, password: string) {
    const authResult = await (this.http.post('/api/auth/sign-in', { email: email, password: password}).toPromise()) as any;
    if (!authResult.success) {
      return false;
    }

    await this.userService.loadCurrentUserIfNeeded(true);
    await this.router.navigate([this.redirectUrl || '/']);
    return true;
  }

  get currentUser() {
    return this.userService.currentUser;
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
    this.userService.unsetCurrentUser();
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
