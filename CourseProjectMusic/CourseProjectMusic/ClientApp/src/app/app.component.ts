import { Component, Inject } from '@angular/core';
import { AuthService, ACCESS_TOKEN } from './services/auth.service';
import { error } from '@angular/compiler/src/util';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { API_URL } from './app-injection-tokens';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html'
})
export class AppComponent {

  constructor() { }
}
