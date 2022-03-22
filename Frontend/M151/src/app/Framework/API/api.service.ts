import { HttpClient } from '@angular/common/http';
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
    const requestPayload = (method === 'GET' || method === 'DELETE') ? JSON.stringify(payload) : payload;
    const requestEndpoint = `${appConfig.API_URL}${endpoint}`;
    let request: any;
    switch (method) {
      case 'GET':
        request = this.http.get(requestEndpoint, requestPayload);
        break;
      case 'POST':
        request = this.http.post(requestEndpoint, requestPayload);
        break;
      case 'PUT':
        request = this.http.put(requestEndpoint, requestPayload);
        break;
      case 'DELETE':
        request = this.http.delete(requestEndpoint, requestPayload);
        break;
    }

    return (request as Observable<T | CustomErrorDTO>).pipe(
      map(result => {
        const error = result as CustomErrorDTO;
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