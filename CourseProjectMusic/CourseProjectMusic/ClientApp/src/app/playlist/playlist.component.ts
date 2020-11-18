import { Component, OnInit, Inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { API_URL } from '../app-injection-tokens';
import { MusicInfo } from '../models/music_info';
import { Router } from '@angular/router';
import { MusicService } from '../services/music.service';

@Component({
  selector: 'app-playlist',
  templateUrl: './playlist.component.html',
  styleUrls: ['./playlist.component.css']
})
export class PlaylistComponent implements OnInit {

  constructor(private http: HttpClient, private musicService: MusicService, private router: Router) { }

  files: MusicInfo[] = [];

  ngOnInit() {
    //Поменять статический id!!!
    this.musicService.getListMusicByUserId(2).subscribe(result => {
      this.files = result;
    }, error => {
        alert(error.message)
    })
  }

  btnAddMusicClick() {
    this.router.navigate(['addmusic']);
  }

}
