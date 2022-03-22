import { Component } from '@angular/core';
import { FormBuilder } from '@angular/forms';
import { Router } from '@angular/router';
import { map } from 'rxjs';
import { appRoutes } from 'src/app/Config/appRoutes';
import { endpoints } from 'src/app/Config/endpoints';
import { LoginDTO } from 'src/app/DTOs/LoginDTO';
import { TokenDTO } from 'src/app/DTOs/TokenDTO';
import { ApiService } from 'src/app/Framework/API/api.service';
import { TokenService } from 'src/app/Framework/API/token.service';
import { FormGroupTyped } from 'src/app/Material/types';

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.scss']
})
export class LoginComponent {

  form: FormGroupTyped<LoginDTO>;
  loginWrong: string | null = null;

  constructor(
    private api: ApiService,
    private formBuilder: FormBuilder,
    private tokenService: TokenService,
    private router: Router,
  ) { 
    this.form = this.formBuilder.group({
      username: '',
      password: '',
    }) as FormGroupTyped<LoginDTO>;
  }

  login() {
    this.api.callApi<TokenDTO>(endpoints.Login, {
      ...this.form.value
    }, 'POST').subscribe(token => {
      if (typeof(token) !== 'string') {
        this.tokenService.setToken(token.token);
        this.tokenService.setRefreshToken(token.refreshToken);
        this.router.navigate([appRoutes.App, appRoutes.MyRuns]);
      } else {
        this.loginWrong = token;
      }
    });
  }

  register() {
    this.router.navigate([appRoutes.Register]);
  }
}
