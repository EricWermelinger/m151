import { Component } from '@angular/core';
import { FormBuilder } from '@angular/forms';
import { endpoints } from 'src/app/Config/endpoints';
import { UserDTO } from 'src/app/DTOs/UserDTO';
import { ApiService } from 'src/app/Framework/API/api.service';
import { FormGroupTyped } from 'src/app/Material/types';

@Component({
  selector: 'app-user-data',
  templateUrl: './user-data.component.html',
  styleUrls: ['./user-data.component.scss']
})
export class UserDataComponent {

  form: FormGroupTyped<UserDTO>;
  showSpinner = false;
  maxDate = new Date();
  minDate = new Date(1900, 0, 1);

  constructor(
    private api: ApiService,
    private formBuilder: FormBuilder,
  ) {
    this.form = this.formBuilder.group({
      firstname: null,
      lastname: null,
      weight: null,
      height: null,
      birthdate: new Date(),
      sexId: null
    }) as FormGroupTyped<UserDTO>;

    this.showSpinner = true;
    this.api.callApi<UserDTO>(endpoints.UserData, {}, 'GET').subscribe(userData => {
      if (typeof(userData) !== 'string') {
        this.form.patchValue(userData);
      }
      this.showSpinner = false;
    });
  }

  save() {
    this.showSpinner = true;
    this.api.callApi(endpoints.UserData, {
      ...this.form.value
    }, 'PUT').subscribe(_ =>
      this.showSpinner = false
    );
  }

  cancel() {
    this.showSpinner = true;
    this.api.callApi<UserDTO>(endpoints.UserData, {}, 'GET').subscribe(userData => {
      if (typeof(userData) !== 'string') {
        this.form.patchValue(userData);
      }
      this.showSpinner = false;
    });
  }
}
