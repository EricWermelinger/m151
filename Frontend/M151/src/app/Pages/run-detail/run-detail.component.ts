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
  showSpinner = false;
  maxDate = new Date();
  minDate = new Date(1900, 0, 1);

  constructor(
    private api: ApiService,
    private formBuilder: FormBuilder,
    private activatedRoute: ActivatedRoute,
    private router: Router,
  ) { 
    this.form = this.formBuilder.group({
      title: null,
      startTime: null,
      length: null,
      duration: null,
      altitude: null,
      gpxFileId: null
    }) as FormGroupTyped<RunDTO>;

    this.activatedRoute.url.pipe(
      map(url => url[1].path)
    ).subscribe(id => this.id$.next(id));

    this.showSpinner = true;
    this.id$.asObservable().pipe(
      filter(id => !!id),
      distinctUntilChanged(),
      switchMap(id => this.api.callApi<RunDTO>(endpoints.RunDetail, id, 'GET')),
    ).subscribe(run => {
      if (typeof(run) !== 'string') {
        this.form.patchValue(run);
      }
      this.showSpinner = false;
    });
  }

  save() {
    const id = this.id$.value;
    if (!!id) {
      this.showSpinner = true;
      this.api.callApi(endpoints.MyRuns, {
        ...this.form.value,
        id,
      }, 'POST').subscribe(_ => this.showSpinner = false);
    }
  }

  cancel() {
    const id = this.id$.value;
    if (!!id) {
      this.showSpinner = true;
      this.api.callApi<RunDTO>(endpoints.RunDetail, id, 'GET').subscribe(run => {
        console.log(run)
        if (typeof(run) !== 'string') {
          this.form.patchValue(run);
        }
        this.showSpinner = false;
      });
    }
  }

  back() {
    this.router.navigate([appRoutes.App, appRoutes.MyRuns]);
  }
}
