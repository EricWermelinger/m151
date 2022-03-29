import { Options } from '@angular-slider/ngx-slider';
import { HttpParams } from '@angular/common/http';
import { Component, OnInit } from '@angular/core';
import { FormBuilder } from '@angular/forms';
import { endpoints } from 'src/app/Config/endpoints';
import { RunDTO } from 'src/app/DTOs/RunDTO';
import { RunFilterDTO } from 'src/app/DTOs/RunFilterDTO';
import { ApiService } from 'src/app/Framework/API/api.service';
import { FormatSeconds } from 'src/app/Framework/Helpers/helpers';

@Component({
  selector: 'app-all-runs',
  templateUrl: './all-runs.component.html',
  styleUrls: ['./all-runs.component.scss']
})
export class AllRunsComponent {

  runs: RunDTO[] = [];
  cities: City[] = [
    { name: 'Aarau', latitude: 47.39, longitude: 8.03 },
    { name: 'Baden', latitude: 47.28, longitude: 8.18 },
    { name: 'Basel', latitude: 47.57, longitude: 7.58 },
    { name: 'Bern', latitude: 46.94, longitude: 7.44 },
    { name: 'Biel/Bienne', latitude: 47.14, longitude: 7.25 },
    { name: 'Chur', latitude: 46.82, longitude: 9.53 },
    { name: 'Genf', latitude: 46.20, longitude: 6.14 },
    { name: 'Lugano', latitude: 46.00, longitude: 8.97 },
    { name: 'Luzern', latitude: 47.02, longitude: 8.32 },
    { name: 'Winterthur', latitude: 47.48, longitude: 8.72 },
    { name: 'Solothurn', latitude: 47.20, longitude: 7.53 },
    { name: 'St. Gallen', latitude: 47.42, longitude: 9.37 },
    { name: 'Thun', latitude: 46.76, longitude: 7.65 },
    { name: 'Zürich', latitude: 47.37, longitude: 8.53 },
  ];
  selectedCity: City = this.cities.filter(c => c.name === 'Baden')[0];
  filter: RunFilterDTO & {city: City} = {
    city: this.selectedCity,
    altitudeMin: 0,
    altitudeMax: 1000,
    lengthMin: 0,
    lengthMax: 100000,
    radiusFromPoint: 10000,
    pointLatitude: this.selectedCity.latitude,
    pointLongitude: this.selectedCity.longitude
  };
  optionsAltitude: Options = {
    floor: 0,
    ceil: 10000
  };
  optionsLength: Options = {
    floor: 0,
    ceil: 100000
  };
  optionsRadius: Options = {
    floor: 0,
    ceil: 100000
  };

  constructor(
    private api: ApiService,
  ) { 
    this.loadData();
  }

  changeCity(city: City) {
    this.filter.pointLatitude = city.latitude;
    this.filter.pointLongitude = city.longitude;
    this.loadData();
  };

  loadData(){
    const filterValue = this.filter;
    if (filterValue.lengthMax >= filterValue.lengthMin && filterValue.altitudeMax >= filterValue.altitudeMin && filterValue.radiusFromPoint >= 0
      && filterValue.pointLatitude > 0 && filterValue.pointLongitude > 0 && filterValue.pointLatitude <= 90 && filterValue.pointLongitude <= 180
      && filterValue.lengthMin >= 0 && filterValue.altitudeMin >= 0) {
        this.api.callApi<RunDTO[]>(endpoints.AllRuns, 
          new HttpParams()
            .set('altitudeMin', filterValue.altitudeMin.toString())
            .set('altitudeMax', filterValue.altitudeMax.toString())
            .set('lengthMin', filterValue.lengthMin.toString())
            .set('lengthMax', filterValue.lengthMax.toString())
            .set('radiusFromPoint', filterValue.radiusFromPoint.toString())
            .set('pointLatitude', filterValue.pointLatitude.toString())
            .set('pointLongitude', filterValue.pointLongitude.toString())     
          , 'GET').subscribe(runs => {
          if (typeof(runs) !== 'string') {
            this.runs = runs;
          }
        });
    }   
  }

  formatSeconds(seconds: number): string {
    return FormatSeconds(seconds);
  }
}

interface City {
  name: string;
  latitude: number;
  longitude: number;
}
