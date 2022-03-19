import { Injectable } from '@angular/core';
import {
  HttpRequest,
  HttpHandler,
  HttpEvent,
  HttpInterceptor
} from '@angular/common/http';
import { catchError, map, Observable, of, switchMap, tap } from 'rxjs';
import { ErrorHandlingService } from './error-handling.service';

@Injectable()
export class ErrorHandlerInterceptor implements HttpInterceptor {

  // todo: add Token & Check in separate service and use in Guard
  checkToken(): Observable<boolean> {
    return of(true);
  }

  // todo: add refresh Token call in separate service
  refreshToken(): Observable<string>{
    return of('token');
  }
  
  constructor(
    private errorHandler: ErrorHandlingService,
  ) {}

  intercept(request: HttpRequest<unknown>, next: HttpHandler): Observable<HttpEvent<unknown>> {
    return next.handle(request).pipe(
      catchError(error => {
        if (error.status === 401){
          return this.refreshToken().pipe(
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
