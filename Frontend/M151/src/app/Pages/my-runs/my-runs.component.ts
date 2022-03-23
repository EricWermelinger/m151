import { Component } from '@angular/core';
import { Router } from '@angular/router';
import { appRoutes } from 'src/app/Config/appRoutes';
import { endpoints } from 'src/app/Config/endpoints';
import { RunDTO } from 'src/app/DTOs/RunDTO';
import { ApiService } from 'src/app/Framework/API/api.service';
import { UUID } from 'angular2-uuid';
import { Observable } from 'rxjs/internal/Observable';

@Component({
  selector: 'app-my-runs',
  templateUrl: './my-runs.component.html',
  styleUrls: ['./my-runs.component.scss']
})
export class MyRunsComponent {

  myRuns$: Observable<RunDTO[]>;

  constructor(
    private api: ApiService,
    private router: Router,
  ) { 
    this.myRuns$ = this.api.callApi<RunDTO[]>(endpoints.MyRuns, null, 'GET') as Observable<RunDTO[]>;
  }

  add() {
    this.router.navigate([appRoutes.App, appRoutes.RunDetails, UUID.UUID()]);
  }

  details(runId: string) {
    this.router.navigate([appRoutes.App, appRoutes.RunDetails, runId]);
  }

  delete(runId: string) {
    this.api.callApi(endpoints.MyRuns, runId, 'DELETE').subscribe(_ => {
      this.myRuns$ = this.api.callApi<RunDTO[]>(endpoints.MyRuns, {}, 'GET') as Observable<RunDTO[]>;
    });
  }

  gpxFile(gpxFileId: string) {
    // todo file download
  }
}
