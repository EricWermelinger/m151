import { Injectable } from '@angular/core';
import { appConfig } from 'src/app/Config/appConfig';

@Injectable({
  providedIn: 'root'
})
export class TokenService {

  constructor() { }

  setToken(token: string) {
    localStorage.setItem(appConfig.TOKEN, token);
  }

  getToken(): string | null {
    return localStorage.getItem(appConfig.TOKEN);
  }

  setRefreshToken(refreshToken: string) {
    localStorage.setItem(appConfig.REFRESH_TOKEN, refreshToken);
  }

  getRefreshToken(): string | null {
    return localStorage.getItem(appConfig.REFRESH_TOKEN);
  }
}
