import { Component } from '@angular/core';
import { TranslateService } from '@ngx-translate/core';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.scss']
})
export class AppComponent {
  title = 'M151';

  constructor(
    private translateService: TranslateService
  ) { }

  // todo: move to navbar
  // todo: fix translations
  selectLanguage(language: string) {
    this.translateService.use(language);
  }
}
