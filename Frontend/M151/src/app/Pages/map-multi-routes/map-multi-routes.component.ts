import { AfterViewInit, Component, Inject, Input, OnInit } from '@angular/core';
import { MAT_DIALOG_DATA } from '@angular/material/dialog';
import * as L from 'leaflet';
import { filter, first, map, Observable } from 'rxjs';
import { endpoints } from 'src/app/Config/endpoints';
import { GpxFileIdsDTO } from 'src/app/DTOs/GpxFileIdsDTO';
import { GpxNodeDTO } from 'src/app/DTOs/GpxNodeDTO';
import { ApiService } from 'src/app/Framework/API/api.service';
import { RandomColor } from 'src/app/Framework/Helpers/helpers';

@Component({
  selector: 'app-map-multi-routes',
  templateUrl: './map-multi-routes.component.html',
  styleUrls: ['./map-multi-routes.component.scss']
})
export class MapMultiRoutesComponent implements OnInit, AfterViewInit {

  routes$?: Observable<GpxNodeDTO[][]>;
  map: any;
  initPassed = false;

  constructor(
    private api: ApiService,
    @Inject(MAT_DIALOG_DATA) private data: MapMultiRoutesComponentData,
  ) { }

  ngOnInit(): void {
    if (this.data.fileIds !== undefined && this.data.fileIds.length > 0) {
      this.routes$ = this.api.callApi<GpxNodeDTO[][]>(endpoints.AllRuns, {
        fileIds: this.data.fileIds
      } as GpxFileIdsDTO, 'POST').pipe(
        filter(files => typeof(files) !== 'string'),
        first(),
      ) as Observable<GpxNodeDTO[][]>;
    }
  }
  
  ngAfterViewInit(): void {
    this.setupMap();
    this.initPassed = true;
  }

  private setupMap() {
    if (this.routes$ !== undefined) {
      this.routes$!.subscribe(routes => {
        this.initializeMap(routes);
      });
    }
  }

  private initializeMap(routes: GpxNodeDTO[][]) {
    const allNodes = routes.flatMap(r => r);
    const latitudeCenter = allNodes.reduce((acc, node) => acc + node.latitude, 0) / allNodes.length;
    const longitudeCenter = allNodes.reduce((acc, node) => acc + node.longitude, 0) / allNodes.length;
    this.map = L.map('map').setView([latitudeCenter, longitudeCenter], 15);

    L.tileLayer('https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png', {
      maxZoom: 18,
      tileSize: 512,
      zoomOffset: -1
    }).addTo(this.map);

    for (let i = 0; i < routes.length; i++) {
      L.polyline(routes[i].map(node => [node.latitude, node.longitude]), {
        color: RandomColor(),
        weight: 10,
        opacity: 1,
        smoothFactor: 1
      }).addTo(this.map);
    }

    const bounds = L.polyline(allNodes.map(node => [node.latitude, node.longitude]), {}).getBounds();

    this.map.fitBounds(bounds);
  }
}

export interface MapMultiRoutesComponentData {
  fileIds: string[];
};