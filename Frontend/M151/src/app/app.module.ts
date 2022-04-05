import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';

import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { MaterialModule } from './Material/material.module';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { HttpClient, HttpClientModule, HTTP_INTERCEPTORS } from '@angular/common/http';
import { ErrorHandlerInterceptor } from './Framework/API/error-handler.interceptor';
import { ErrorHandlingDialogComponent } from './Framework/error-handling-dialog/error-handling-dialog.component';
import { LoginComponent } from './Pages/login/login.component';
import { RegisterComponent } from './Pages/register/register.component';
import { TranslateModule, TranslateLoader } from '@ngx-translate/core';
import { TranslateHttpLoader } from '@ngx-translate/http-loader';
import { UserDataComponent } from './Pages/user-data/user-data.component';
import { SpinnerDirective } from './Framework/spinner/spinner.directive';
import { MyRunsComponent } from './Pages/my-runs/my-runs.component';
import { RunDetailComponent } from './Pages/run-detail/run-detail.component';
import { NavBarComponent } from './Pages/index/nav-bar.component';
import { AllRunsComponent } from './Pages/all-runs/all-runs.component';
import { MapsComponent } from './Pages/maps/maps.component';
import { MapMultiRoutesComponent } from './Pages/map-multi-routes/map-multi-routes.component';

export function HttpLoaderFactory(http: HttpClient) {
  return new TranslateHttpLoader(http, './assets/i18n/', '.json');
}

@NgModule({
  declarations: [
    AppComponent,
    ErrorHandlingDialogComponent,
    LoginComponent,
    RegisterComponent,
    UserDataComponent,
    SpinnerDirective,
    MyRunsComponent,
    RunDetailComponent,
    NavBarComponent,
    AllRunsComponent,
    MapsComponent,
    MapMultiRoutesComponent,
  ],
  imports: [
    BrowserModule,
    AppRoutingModule,
    BrowserAnimationsModule,
    MaterialModule,
    FormsModule,
    ReactiveFormsModule,
    HttpClientModule,
    TranslateModule.forRoot({
      defaultLanguage: 'en-GB',
      loader: {
        provide: TranslateLoader,
        useFactory: HttpLoaderFactory,
        deps: [HttpClient]
      }
    }),
  ],
  providers: [
    {
      provide: HTTP_INTERCEPTORS,
      useClass: ErrorHandlerInterceptor,
      multi: true
    },
  ],
  bootstrap: [AppComponent]
})
export class AppModule { }
