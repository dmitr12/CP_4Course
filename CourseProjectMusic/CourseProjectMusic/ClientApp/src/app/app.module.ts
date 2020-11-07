import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { HttpClientModule, HTTP_INTERCEPTORS } from '@angular/common/http';
import { RouterModule } from '@angular/router';

import { AppComponent } from './app.component';
import { NavMenuComponent } from './nav-menu/nav-menu.component';
import { HomeComponent } from './home/home.component';
import { CounterComponent } from './counter/counter.component';
import { FetchDataComponent } from './fetch-data/fetch-data.component';
import { JwtModule } from '@auth0/angular-jwt'
import { API_URL } from './app-injection-tokens';
import { environment } from '../environments/environment';
import { ACCESS_TOKEN } from './services/auth.service';
import { LoginComponent } from './login/login.component';
import { AuthlayoutComponent } from './layouts/authlayout/authlayout.component';
import { RegisterComponent } from './register/register.component';
import { log } from 'util';

export function tokenGetter() {
  return localStorage.getItem(ACCESS_TOKEN);
}

@NgModule({
  declarations: [
    AppComponent,
    NavMenuComponent,
    HomeComponent,
    CounterComponent,
    FetchDataComponent,
    LoginComponent,
    AuthlayoutComponent,
    RegisterComponent
  ],
  imports: [
    BrowserModule.withServerTransition({ appId: 'ng-cli-universal' }),
    HttpClientModule,
    FormsModule,
    ReactiveFormsModule,
    JwtModule.forRoot({
      config: {
        tokenGetter/*,*/
        //whitelistedDomains: environment.tokenWhiteListedDomains
      }
    }),
    RouterModule.forRoot([
      {
        path: '', component: AuthlayoutComponent, children: [
          { path: '', redirectTo: '/login', pathMatch: 'full' },
          { path: 'login', component: LoginComponent },
          { path: 'register', component: RegisterComponent }
        ]
      }
      //{ path: '', component: HomeComponent, pathMatch: 'full' },
      //{ path: 'counter', component: CounterComponent },
      //{ path: 'fetch-data', component: FetchDataComponent },
      //{ path: 'login', component: LoginComponent },
    ])
  ],
  providers: [{
    provide: API_URL,
    useValue: environment.api
  }],
  bootstrap: [AppComponent]
})
export class AppModule { }
