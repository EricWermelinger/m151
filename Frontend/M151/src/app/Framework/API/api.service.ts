import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { map, Observable } from 'rxjs';
import { CustomErrorDTO } from 'src/app/DTOs/CustomErrorDTO';
import { appConfig } from '../../Config/appConfig';

@Injectable({
  providedIn: 'root'
})
export class ApiService {
  constructor(
    private http: HttpClient,
  ) { }

  callApi<T>(endpoint: string, payload: any, method: HttpMethods) {
    const requestEndpoint = `${appConfig.API_URL}${endpoint}`;
    const params = ((method === 'GET' || method === 'DELETE') && !!payload) ? new HttpParams().set('id', payload) : {};
    let request: any;
    switch (method) {
      case 'GET':
        request = this.http.get(requestEndpoint, { params });
        break;
      case 'POST':
        request = this.http.post(requestEndpoint, payload);
        break;
      case 'PUT':
        request = this.http.put(requestEndpoint, payload);
        break;
      case 'DELETE':
        request = this.http.delete(requestEndpoint, { params });
        break;
    }

    return (request as Observable<T | CustomErrorDTO>).pipe(
      map(result => {
        const error = result as CustomErrorDTO;
        if (!error) {
          return null;
        }
        if (error.errorKey !== undefined) {
          return 'error.' + error.errorKey;
        } else {
          return result as T;
        }
      }),
    ) as Observable<T | string>;
  }
}

export type HttpMethods = 'GET' | 'POST' | 'PUT' | 'DELETE';