import { Injectable } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { Observable } from 'rxjs';
import { appConfig } from 'src/app/Config/appConfig';
import { appRoutes } from 'src/app/Config/appRoutes';
import { ErrorHandlingDialogComponent } from '../error-handling-dialog/error-handling-dialog.component';

@Injectable({
  providedIn: 'root'
})
export class ErrorHandlingService {

  constructor(
    private dialog: MatDialog,
  ) { }

  handleError(data: any): Observable<any> {
    const ref = this.dialog.open(ErrorHandlingDialogComponent, {
      data,
      disableClose: true,
    });
    return ref.afterClosed();
  }

  redirectToLogin() {
    self.location.href = `${appConfig.FRONTEND_URL}${appRoutes.Login}`;
  }
}
