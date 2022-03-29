import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { appRoutes } from './Config/appRoutes';
import { AppGuard } from './Framework/API/app.guard';
import { AllRunsComponent } from './Pages/all-runs/all-runs.component';
import { LoginComponent } from './Pages/login/login.component';
import { MyRunsComponent } from './Pages/my-runs/my-runs.component';
import { RegisterComponent } from './Pages/register/register.component';
import { RunDetailComponent } from './Pages/run-detail/run-detail.component';
import { UserDataComponent } from './Pages/user-data/user-data.component';

const routes: Routes = [
  { path: appRoutes.Login, component: LoginComponent },
  { path: appRoutes.Register, component: RegisterComponent },
  {
    path: appRoutes.App,
    canActivate: [AppGuard],
    children: [
      { path: appRoutes.UserData, component: UserDataComponent, pathMatch: 'full' },
      { path: appRoutes.MyRuns, component: MyRunsComponent, pathMatch: 'full' },
      { path: `${appRoutes.RunDetails}/:id`, component: RunDetailComponent, pathMatch: 'full' }
    ],
  },
  { path: appRoutes.AllRuns, component: AllRunsComponent },
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
