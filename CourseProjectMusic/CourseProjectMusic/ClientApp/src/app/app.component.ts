import { Component } from '@angular/core';
import { AuthService } from './services/auth.service';
import { error } from '@angular/compiler/src/util';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html'
})
export class AppComponent {

  constructor() { }

  files = [{
    url: 'https://music1storage.blob.core.windows.net/music/4.mp3',
    name: '4.mp3'
  }, {
    url: 'https://music1storage.blob.core.windows.net/music/3.mp3',
    name: '3.mp3'
  }, {
    url: 'https://music1storage.blob.core.windows.net/music/1.mp3',
    name: '1.mp3'
  }];
}
