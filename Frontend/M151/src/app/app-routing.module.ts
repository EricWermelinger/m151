import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { appRoutes } from './Config/appRoutes';
import { AppGuard } from './Framework/API/app.guard';
import { TestComponentComponent } from './test-component/test-component.component';

const routes: Routes = [
  { path: appRoutes.Login, component: TestComponentComponent },
  {
    path: appRoutes.App,
    canActivate: [AppGuard],
    children: [
      
    ],
  },
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
