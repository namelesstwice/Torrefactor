import { Injectable } from '@angular/core';
import {User} from './user';
import {HttpClient} from '@angular/common/http';
import {BehaviorSubject, Observable, of} from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class UserService {

  constructor(
    private http: HttpClient,
  ) { }

  private isUserFetched = false;
  private readonly _userObservable: BehaviorSubject<User> = new BehaviorSubject(null);
  public readonly userObservable = this._userObservable.asObservable();

  get currentUser() {
    if (!this.isUserFetched) {
      throw new Error('You should call `loadCurrentUserIfNeeded` first');
    }

    return this._userObservable.getValue();
  }

  public unsetCurrentUser() {
    this._userObservable.next(null);
  }

  public async loadCurrentUserIfNeeded(force?: boolean) {
    if (this.isUserFetched && !force) {
      return;
    }

    const user = await (this.http.get<User>('/api/auth/user')).toPromise();
    this._userObservable.next(user);
    this.isUserFetched = true;
  }
}
