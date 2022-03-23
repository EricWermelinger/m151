import { Component, OnInit } from '@angular/core';
import { FormBuilder } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { BehaviorSubject, distinctUntilChanged, filter, map, Observable, switchMap } from 'rxjs';
import { appRoutes } from 'src/app/Config/appRoutes';
import { endpoints } from 'src/app/Config/endpoints';
import { RunDTO } from 'src/app/DTOs/RunDTO';
import { ApiService } from 'src/app/Framework/API/api.service';
import { FormGroupTyped } from 'src/app/Material/types';

@Component({
  selector: 'app-run-detail',
  templateUrl: './run-detail.component.html',
  styleUrls: ['./run-detail.component.scss']
})
export class RunDetailComponent {

  form: FormGroupTyped<RunDTO>;
  id$ = new BehaviorSubject<string | null>(null);

  constructor(
    private api: ApiService,
    private formBuilder: FormBuilder,
    private activatedRoute: ActivatedRoute,
    private router: Router,
  ) { 
    this.form = this.formBuilder.group({
      title: null,
      starttime: null,
      length: null,
      duration: null,
      altitude: null,
      gpxFileId: null
    }) as FormGroupTyped<RunDTO>;

    this.activatedRoute.url.pipe(
      map(url => url[1].path)
    ).subscribe(id => this.id$.next(id));

    this.id$.asObservable().pipe(
      filter(id => !!id),
      distinctUntilChanged(),
      switchMap(id => this.api.callApi<RunDTO>(endpoints.RunDetail, id, 'GET')),
    ).subscribe(run => {
      if (typeof(run) !== 'string') {
        this.form.patchValue(run);
      }
    });
  }

  save() {
    const id = this.id$.value;
    if (!!id) {
      this.api.callApi(endpoints.MyRuns, {
        ...this.form.value,
        id,
      }, 'POST');
    }
  }

  cancel() {
    const id = this.id$.value;
    if (!!id) {
      this.api.callApi<RunDTO>(endpoints.RunDetail, id, 'GET').subscribe(run => {
        if (typeof(run) !== 'string') {
          this.form.patchValue(run);
        }
      });
    }
  }

  back() {
    this.router.navigate([appRoutes.App, appRoutes.MyRuns]);
  }
}
