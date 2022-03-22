import { Component, OnInit } from '@angular/core';
import { FormBuilder } from '@angular/forms';
import { Router } from '@angular/router';
import { appRoutes } from 'src/app/Config/appRoutes';
import { endpoints } from 'src/app/Config/endpoints';
import { LoginDTO } from 'src/app/DTOs/LoginDTO';
import { TokenDTO } from 'src/app/DTOs/TokenDTO';
import { ApiService } from 'src/app/Framework/API/api.service';
import { TokenService } from 'src/app/Framework/API/token.service';
import { FormGroupTyped } from 'src/app/Material/types';

@Component({
  selector: 'app-register',
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.scss']
})
export class RegisterComponent {

  form: FormGroupTyped<LoginDTO>;
  registerWrong: string | null = null;

  constructor(
    private api: ApiService,
    private formBuilder: FormBuilder,
    private tokenService: TokenService,
    private router: Router,
  ) { 
    this.form = this.formBuilder.group({
      username: '',
      password: ''
    }) as FormGroupTyped<LoginDTO>;
  }

  register(){
    this.api.callApi<TokenDTO>(endpoints.Register, {
      ...this.form.value
    }, 'POST').subscribe(token => {
      if (typeof(token) !== 'string') {
        this.tokenService.setToken(token.token);
        this.tokenService.setRefreshToken(token.refreshToken);
        this.router.navigate([appRoutes.App, appRoutes.MyRuns]);
      } else {
        this.registerWrong = token;
      }
    });
  }

  login() {
    this.router.navigate([appRoutes.Login]);
  }
}
