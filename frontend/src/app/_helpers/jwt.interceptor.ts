import { Injectable } from '@angular/core';
import { HttpRequest, HttpHandler, HttpEvent, HttpInterceptor } from '@angular/common/http';
import { Observable } from 'rxjs';
import { AuthService } from '../_services/auth-service.service';

@Injectable()
export class JwtInterceptor implements HttpInterceptor {
    constructor(private auth: AuthService) { }

    intercept(request: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
        const isApiUrl = request.url.startsWith('/api');

        if (this.auth.isAuthenticated && isApiUrl) {
            request = request.clone({
                setHeaders: {
                    Authorization: `Bearer ${this.auth.currentTokenValue}`
                }
            });
        }

        return next.handle(request);
    }
}
