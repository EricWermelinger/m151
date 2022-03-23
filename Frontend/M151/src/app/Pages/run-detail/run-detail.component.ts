import { Component, OnInit } from '@angular/core';
import { FormBuilder } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { UUID } from 'angular2-uuid';
import { BehaviorSubject, debounceTime, distinctUntilChanged, filter, map, Observable, switchMap, tap } from 'rxjs';
import { appRoutes } from 'src/app/Config/appRoutes';
import { endpoints } from 'src/app/Config/endpoints';
import { RunDTO } from 'src/app/DTOs/RunDTO';
import { RunNoteDTO } from 'src/app/DTOs/RunNoteDTO';
import { ApiService } from 'src/app/Framework/API/api.service';
import { FormGroupTyped } from 'src/app/Material/types';

@Component({
  selector: 'app-run-detail',
  templateUrl: './run-detail.component.html',
  styleUrls: ['./run-detail.component.scss']
})
export class RunDetailComponent {

  // todo icons for add / edit / delete

  form: FormGroupTyped<RunDTO>;
  id$ = new BehaviorSubject<string | null>(null);
  showSpinner = false;
  maxDate = new Date();
  minDate = new Date(1900, 0, 1);
  notes$ = new BehaviorSubject<FormGroupTyped<RunNoteDTO>[]>([]);
  isSet = false;

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
      if (typeof(run) !== 'string' && run !== null) {
        this.form.patchValue(run);
        this.isSet = true;
      }
      this.showSpinner = false;
    });

    this.id$.asObservable().pipe(
      filter(id => !!id),
      distinctUntilChanged(),
      switchMap(id => this.api.callApi<RunNoteDTO[]>(endpoints.Notes, id, 'GET')),
    ).subscribe(notes => {
      if (typeof(notes) !== 'string' && notes !== null) {
        const formGroups = notes.map(note => this.formBuilder.group({
          id: note.id,
          note: note.note,
          runId: note.runId
        }) as FormGroupTyped<RunNoteDTO>);
        this.notes$.next(formGroups);
        this.notes$.value.forEach(note => note.valueChanges.pipe(
          distinctUntilChanged(),
          debounceTime(500),
        ).subscribe(noteValue => {
          this.handleNoteChange(noteValue);
        }));
      }
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
  
  addNote() {
    const runId = this.id$.value;
    if (!!runId) {
      const newFormGroup = this.formBuilder.group({
        id: UUID.UUID(),
        note: '',
        runId 
      }) as FormGroupTyped<RunNoteDTO>;
      newFormGroup.valueChanges.pipe(
        distinctUntilChanged(),
        debounceTime(500),
      ).subscribe(noteValue => {
        this.handleNoteChange(noteValue);
      });
      this.notes$.next([...this.notes$.value, newFormGroup]);
    }
  }

  private saveNote(formValue: RunNoteDTO) {
    this.api.callApi(endpoints.Notes, formValue, 'POST').subscribe();
  }

  private handleNoteChange(noteValue: RunNoteDTO) {
    if (noteValue.note !== null && noteValue.note !== '') {
      this.saveNote(noteValue);
    } else {
      this.deleteNote(noteValue.id);
    }
  }

  deleteNote(noteId: string) {
    this.api.callApi(endpoints.Notes, noteId, 'DELETE').subscribe();
    this.notes$.next([...this.notes$.value.filter(x => x.value.id !== noteId)]);
  }
}
