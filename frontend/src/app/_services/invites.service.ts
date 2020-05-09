import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { User } from '../_models/user';

@Injectable({
  providedIn: 'root'
})
export class InvitesService {

  constructor(private http: HttpClient) { }

  public getInvites() {
    return this.http.get<User[]>('/api/auth/users/not-confirmed');
  }

  public accept(user: User) {
    return this.http.put(`/api/auth/users/${user.id}/confirmation-state?isApproved=true`, {});
  }

  public decline(user: User) {
    return this.http.put(`/api/auth/users/${user.id}/confirmation-state?isApproved=false`, {});
  }
}
