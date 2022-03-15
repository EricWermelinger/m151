import { Injectable } from '@angular/core';
import {
  HttpRequest,
  HttpHandler,
  HttpEvent,
  HttpInterceptor
} from '@angular/common/http';
import { catchError, map, Observable, of, switchMap, tap } from 'rxjs';
import { appConfig } from '../Config/appConfig';
import { appRoutes } from '../Config/appRoutes';

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

  // todo: move to separate service
  redirectToLogin() {
    self.location.href = `${appConfig.FRONTEND_URL}${appRoutes.Login}`;
  }

  // todo: add ErrorHandling (as Dialog) in separate class with dialog
  handleError(error: any, payload: any): Observable<any>{
    return of(null);
  }
  
  constructor() {}

  intercept(request: HttpRequest<unknown>, next: HttpHandler): Observable<HttpEvent<unknown>> {
    return next.handle(request).pipe(
      catchError(error => {
        if (error.status === 401){
          return this.refreshToken().pipe(
            map(token => !token),
            tap(isUnauthenticated => {
              if (isUnauthenticated) {
                this.redirectToLogin();
              }
            }),
            switchMap(_ => next.handle(request))
          );
        } else {
          return this.handleError(error, request);
        }
      }),
    )
  }
}
