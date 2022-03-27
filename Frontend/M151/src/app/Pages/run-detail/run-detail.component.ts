import { Component, OnInit } from '@angular/core';
import { FormBuilder } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { UUID } from 'angular2-uuid';
import { BehaviorSubject, debounceTime, distinctUntilChanged, filter, map, NEVER, never, Observable, switchMap, tap } from 'rxjs';
import { appRoutes } from 'src/app/Config/appRoutes';
import { endpoints } from 'src/app/Config/endpoints';
import { GpxFileDTO } from 'src/app/DTOs/GpxFileDTO';
import { GpxNodeDTO } from 'src/app/DTOs/GpxNodeDTO';
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

  form: FormGroupTyped<RunDTO & { gpxFile: any }>;
  id$ = new BehaviorSubject<string | null>(null);
  showSpinner = false;
  maxDate = new Date();
  minDate = new Date(1900, 0, 1);
  notes$ = new BehaviorSubject<FormGroupTyped<RunNoteDTO>[]>([]);
  isSet = false;
  fileError = false;

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
      gpxFileId: null,
      gpxFile: null
    }) as FormGroupTyped<RunDTO & { gpxFile: any }>;

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
        this.setDisabled(true);
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

  fileChanged(e: any) {
    const file = e.target.files[0];
    let fileReader = new FileReader();
    fileReader.onload = (_) => {
      this.fileError = false;
      const fileContent = fileReader.result as string;
      let parser = new DOMParser();
      if (!fileContent) {
        this.fileError = true;
        return;
      }
      const fileContentAsXML = parser.parseFromString(fileContent, "text/xml");
      const trkpt = fileContentAsXML.getElementsByTagName('trkpt');
      if (!trkpt) {
        this.fileError = true;
        return;
      }
      const nodes: GpxNodeDTO[] = [];
      for (let i = 0; i < trkpt.length; i++) {
        const xmlNode = trkpt[i];
        if (xmlNode.childNodes.length < 2 || !xmlNode.getAttributeNode('lat') || !xmlNode.getAttributeNode('lon')) {
          this.fileError = true;
          return;
        }
        nodes.push({
          elevation: parseFloat(xmlNode.childNodes[0].firstChild?.nodeValue ?? '0'),
          time: new Date(xmlNode.childNodes[1].firstChild?.nodeValue ?? '0'),
          latitude: parseFloat(xmlNode.getAttributeNode('lat')?.nodeValue ?? '0'),
          longitude: parseFloat(xmlNode.getAttributeNode('lon')?.nodeValue ?? '0'),
          orderInFile: i,
        } as GpxNodeDTO);
      }
     
      if (nodes.length < 2) {
        this.fileError = true;
        return;
      }

      console.log(nodes);
      const gpxFile = {
        runId: this.id$.value,
        filename: file.name,
        nodes: nodes
      } as GpxFileDTO;

      // todo find error in calculation
      this.api.callApi<RunDTO>(endpoints.GpxFile, gpxFile, 'POST').subscribe(run => {
        if (typeof(run) !== 'string') {
          this.form.patchValue(run);
          this.setDisabled(true);
          this.fileError = false;
        } else {
          this.fileError = true;
        }        
      });
    }
    fileReader.readAsText(file);
  }

  deleteNote(noteId: string) {
    this.api.callApi(endpoints.Notes, noteId, 'DELETE').subscribe();
    this.notes$.next([...this.notes$.value.filter(x => x.value.id !== noteId)]);
  }

  deleteFile() {
    this.form.controls.gpxFile.patchValue(NEVER);
    this.api.callApi(endpoints.GpxFile, this.form.value.gpxFileId, 'DELETE').subscribe(_ => {
      this.setDisabled(false);
      this.form.controls.gpxFileId.patchValue(null);
    });
  }

  private setDisabled(disabled: boolean){
    if (disabled) {
      this.form.controls.altitude.disable();
      this.form.controls.duration.disable();
      this.form.controls.gpxFile.disable();
      this.form.controls.gpxFileId.disable();
      this.form.controls.length.disable();
      this.form.controls.startTime.disable();
      this.form.controls.title.disable();
    } else {
      this.form.controls.altitude.enable();
      this.form.controls.duration.enable();
      this.form.controls.gpxFile.enable();
      this.form.controls.gpxFileId.enable();
      this.form.controls.length.enable();
      this.form.controls.startTime.enable();
      this.form.controls.title.enable();
    }
    this.form.updateValueAndValidity();
  }
}
