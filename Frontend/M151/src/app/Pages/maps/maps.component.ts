import { AfterViewInit, Component, Input, OnInit } from '@angular/core';
import * as L from 'leaflet';
import { filter, first, map, Observable, Subscriber } from 'rxjs';
import { endpoints } from 'src/app/Config/endpoints';
import { GpxFileDTO } from 'src/app/DTOs/GpxFileDTO';
import { GpxNodeDTO } from 'src/app/DTOs/GpxNodeDTO';
import { ApiService } from 'src/app/Framework/API/api.service';

@Component({
  selector: 'app-maps',
  templateUrl: './maps.component.html',
  styleUrls: ['./maps.component.scss']
})
export class MapsComponent implements OnInit, AfterViewInit {

  @Input() set gpxFileId(id: string | null)  {
    this.id = id;
    this.attributeId = 'map-' + id;
  };
  map: any;
  id: string | null = null;
  attributeId: string = '';
  nodes$?: Observable<GpxNodeDTO[]>;

  constructor(
    private api: ApiService,
  ) { }

  ngOnInit(): void {
    if (this.id !== null) {
      this.nodes$ = this.api.callApi<GpxFileDTO>(endpoints.GpxFile, this.id, 'GET').pipe(
        filter(nodes => typeof(nodes) !== 'string'),
        first(),
        map(nodes => (nodes as GpxFileDTO).nodes),
      ) as Observable<GpxNodeDTO[]>;
    }
  }
  
  ngAfterViewInit(): void {
    this.nodes$!.subscribe(nodes => {
      this.initializeMap(nodes);
    });
  }

  private initializeMap(nodes: GpxNodeDTO[]) {
    const latitudeCenter = nodes.reduce((acc, node) => acc + node.latitude, 0) / nodes.length;
    const longitudeCenter = nodes.reduce((acc, node) => acc + node.longitude, 0) / nodes.length;
    this.map = L.map(this.attributeId).setView([latitudeCenter, longitudeCenter], 15);

    L.tileLayer('https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png', {
      maxZoom: 18,
      tileSize: 512,
      zoomOffset: -1
    }).addTo(this.map);

    const polyline = L.polyline(nodes.map(node => [node.latitude, node.longitude]), {
      color: 'red',
      weight: 3,
      opacity: 2,
      smoothFactor: 1
    });
    polyline.addTo(this.map);

    this.map.fitBounds(polyline.getBounds());
  }
}
