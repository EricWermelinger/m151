import { KeyValue } from '@angular/common';
import { Component } from '@angular/core';
import { TranslateService } from '@ngx-translate/core';
import { TokenService } from './Framework/API/token.service';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.scss']
})
export class AppComponent {
  title = 'M151';

  constructor(
    private tokenService: TokenService,
    private translateService: TranslateService,
  ) {
    if (!this.translateService.currentLang) {
      const tokenLang = this.getLang(this.tokenService.getSelectedLanguage());
      const browserLang = this.getLang(this.translateService.getBrowserLang());
      if (!!tokenLang && !this.translateService.currentLang) {
        this.translateService.use(tokenLang);
      } else if (!!browserLang && !this.translateService.currentLang) {
        this.translateService.use(browserLang);
      } else {
        this.translateService.use(this.translateService.defaultLang);
      }
    }
  }

  getLang(language: string | null | undefined): string | null {
    if (!language) {
      return null;
    }
    if (languages.filter(l => l.key === language).length > 0) {
      return language;
    }
    if (language.length === 2 && languages.map(l => l.key.slice(0, 2)).filter(l => l === language).length > 0) {
      return languages.filter(l => l.key.slice(0, 2) === language)[0].key;
    }
    return null;
  }
}

export const languages: KeyValue<string, string>[] = [
  { key: 'en-GB', value: 'languages.english' },
  { key: 'ar-SA', value: 'languages.arabic' },
  { key: 'zh-CN', value: 'languages.chinese' },
  { key: 'da-DK', value: 'languages.danish' },
  { key: 'nl-NL', value: 'languages.dutch' },
  { key: 'fi-FI', value: 'languages.finnish' },
  { key: 'fr-FR', value: 'languages.french' },
  { key: 'de-DE', value: 'languages.german' },
  { key: 'hi-IN', value: 'languages.hindi' },
  { key: 'it-IT', value: 'languages.italian' },
  { key: 'ja-JP', value: 'languages.japanese' },
  { key: 'ko-KR', value: 'languages.korean' },
  { key: 'pl-PL', value: 'languages.polish' },
  { key: 'pt-PT', value: 'languages.portuguese' },
  { key: 'ru-RU', value: 'languages.russian' },
  { key: 'es-ES', value: 'languages.spanish' },
  { key: 'sv-SE', value: 'languages.swedish' },
  { key: 'th-TH', value: 'languages.thai' },
  { key: 'tr-TR', value: 'languages.turkish' },
  { key: 'vi-VN', value: 'languages.vietnamese' },
];