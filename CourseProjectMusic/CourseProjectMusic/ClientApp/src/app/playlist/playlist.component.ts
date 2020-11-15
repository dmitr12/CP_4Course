import { Component, OnInit } from '@angular/core';

@Component({
  selector: 'app-playlist',
  templateUrl: './playlist.component.html',
  styleUrls: ['./playlist.component.css']
})
export class PlaylistComponent implements OnInit {

  constructor() { }

  ngOnInit() {
  }

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
