import { Component } from '@angular/core';
import { ActivatedRoute, NavigationEnd, Router, UrlSegment } from '@angular/router';
import { TranslateService } from '@ngx-translate/core';
import { combineLatest, distinctUntilChanged, filter, interval, map, Observable, startWith, tap } from 'rxjs'
import { languages } from 'src/app/app.component';
import { TokenService } from 'src/app/Framework/API/token.service';
import { appRoutes } from './../../Config/appRoutes';

@Component({
  selector: 'app-nav-bar',
  templateUrl: './nav-bar.component.html',
  styleUrls: ['./nav-bar.component.scss']
})
export class NavBarComponent {

  routes$: Observable<SelectableRoute[]>;
  selectableLanguages = languages;

  private routes = [
    { route: appRoutes.Login, key: 'login.login', navigation: [appRoutes.Login], onLoggedIn: false },
    { route: appRoutes.Register, key: 'login.register', navigation: [appRoutes.Register], onLoggedIn: false },
    { route: appRoutes.UserData, key: 'userData.userData', navigation: [appRoutes.App, appRoutes.UserData], onLoggedIn: true },
    { route: [appRoutes.MyRuns, appRoutes.RunDetails], key: 'runDetails.myRuns', navigation: [appRoutes.App, appRoutes.MyRuns], onLoggedIn: true },
    { route: appRoutes.AllRuns, key: 'allRuns.allRuns', navigation: [appRoutes.AllRuns], onLoggedIn: null }
  ] as AppRoute[];

  constructor(
    private tokenService: TokenService,
    private translateService: TranslateService,
    private activatedRoute: ActivatedRoute,
    private router: Router,
  ) {
    const loggedIn$ = interval(1000).pipe(
      startWith(false),
      map(_ => !!this.tokenService.getToken()),
      distinctUntilChanged(),
    );

    this.routes$ = combineLatest([
      loggedIn$,
      this.router.events.pipe(
        filter(event => event instanceof NavigationEnd),
        map(event => (event as NavigationEnd).url),
        map(url => url.split('/')),
      )
    ]).pipe(
      map(([isLoggedIn, url]) => [isLoggedIn, this.getActiveRoute(url)]),
      map(([isLoggedIn, url]) =>
        this.routes
          .filter(r => r.onLoggedIn === null || r.onLoggedIn === isLoggedIn)
          .map(r => {
              return {
              route: r,
              selected: url === r.route
            } as SelectableRoute;
          })
      )
    );
  }

  navigate(route: string[]) {
    this.router.navigate(route);
  }

  selectLanguage(language: string) {
    this.translateService.use(language);
    this.tokenService.setSelectedLanguage(language);
  }

  getSelectedLanguage(): string {
    return this.translateService.currentLang ?? this.translateService.defaultLang;
  }

  private getActiveRoute(url: string[]){
    const urlParts = url.filter(segment => segment !== '' && segment !== appRoutes.App);
    const filteredRoutes = this.routes.filter(route => urlParts.some(part => route.route.includes(part)));
    if (filteredRoutes.length > 0) {
      return filteredRoutes[0].route;
    } else {
      return '';
    }
  }
}

interface SelectableRoute {
  route: AppRoute;
  selected: boolean;
};

interface AppRoute {
  route: string | string[];
  key: string;
  navigation: string[];
  onLoggedIn: boolean | null;
}