import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { appRoutes } from './Config/appRoutes';
import { AppGuard } from './Framework/API/app.guard';
import { LoginComponent } from './Pages/login/login.component';
import { RegisterComponent } from './Pages/register/register.component';
import { UserDataComponent } from './Pages/user-data/user-data.component';

const routes: Routes = [
  { path: appRoutes.Login, component: LoginComponent },
  { path: appRoutes.Register, component: RegisterComponent },
  {
    path: appRoutes.App,
    canActivate: [AppGuard],
    children: [
      { path: appRoutes.UserData, component: UserDataComponent, pathMatch: 'full' },
    ],
  },
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
