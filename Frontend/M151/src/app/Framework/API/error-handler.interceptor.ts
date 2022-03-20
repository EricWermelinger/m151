import { Injectable } from '@angular/core';
import {
  HttpRequest,
  HttpHandler,
  HttpEvent,
  HttpInterceptor
} from '@angular/common/http';
import { catchError, map, Observable, of, switchMap, tap } from 'rxjs';
import { ErrorHandlingService } from './error-handling.service';
import { TokenService } from './token.service';
import { appConfig } from 'src/app/Config/appConfig';

@Injectable()
export class ErrorHandlerInterceptor implements HttpInterceptor {
  
  constructor(
    private errorHandler: ErrorHandlingService,
    private tokenService: TokenService,
  ) {}

  intercept(request: HttpRequest<unknown>, next: HttpHandler): Observable<HttpEvent<unknown>> {
    const token = this.tokenService.getToken();
    if (!!token) {
      request = request.clone({
        setHeaders: {
          [appConfig.AUTHORIZATION]: `${appConfig.BEARER} ${token}`
        }
      });
    }

    return next.handle(request).pipe(
      catchError(error => {
        if (error.status === 401){
          return of(this.tokenService.getRefreshToken()).pipe(
            map(token => !token),
            tap(isUnauthenticated => {
              if (isUnauthenticated) {
                this.errorHandler.redirectToLogin();
              }
            }),
            switchMap(_ => next.handle(request))
          );
        } else {
          return this.errorHandler.handleError({
            error,
            request
          });
        }
      }),
    )
  }
}
