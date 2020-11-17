import { Component, OnInit, Inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { API_URL } from '../app-injection-tokens';
import { MusicInfo } from '../models/music_info';

@Component({
  selector: 'app-playlist',
  templateUrl: './playlist.component.html',
  styleUrls: ['./playlist.component.css']
})
export class PlaylistComponent implements OnInit {

  constructor(private http: HttpClient, @Inject(API_URL) private apiUrl: string,) { }

  ngOnInit() {
    this.http.get<MusicInfo[]>(`${this.apiUrl}api/music/list/` + '1').subscribe(result => {
      this.files = result;
    }, error => {
        alert(error)
    })
  }

  files: MusicInfo[] = [];

}
