import { Injectable } from '@angular/core';
import { HttpClient, HttpErrorResponse } from '@angular/common/http';
import { catchError, map } from 'rxjs/operators';
import { throwError, BehaviorSubject, Observable } from 'rxjs';

export class AuthError {
  constructor(public errorText: string) {}
}

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private currentTokenSubject: BehaviorSubject<string>;

  constructor(private http: HttpClient) { 
    this.currentTokenSubject = new BehaviorSubject<string>(localStorage.getItem('jwt'));
  }

  public get currentTokenValue() {
    return this.currentTokenSubject.value;
  }

  public get isAuthenticated() {
    return this.currentTokenSubject.value != null;
  }

  register(name: string, email: string, password: string) {
    return this.http
      .post('/api/auth/register', {
        name,
        email,
        password
      })
      .pipe(catchError((e: HttpErrorResponse) => {
        if (e.error.errors) {
          if (e.error.errors.some(e => e.code === 'DuplicateEmail')) {
            return throwError(new AuthError('User with this email is already registered.'));
          }
        }
        return throwError(new AuthError('Unexpected error, please try again later'));
      }));
  }

  login(email: string, password: string) {
    return this.http
      .post('/api/auth/sign-in', {
        email,
        password
      })
      .pipe(map(auth => {
        const jwt = (<any>auth).accessToken;
        this.currentTokenSubject.next(jwt);
        localStorage.setItem('jwt', jwt);
      }))
      .pipe(catchError((e: HttpErrorResponse) => {
        if (e.status == 401) {
          return throwError(new AuthError(
            e.error.isNotApproved 
              ? 'Your account is not approved yet, please wait' 
              : 'Invalid credentials'));
        }
        return throwError(new AuthError('Unexpected error, please try again later error'));
      }));;
  }

  logout() {
    this.currentTokenSubject.next(null);
    localStorage.removeItem('jwt');
  }
}
