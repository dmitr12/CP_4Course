import { Injectable, Inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { MusicGenreInfo } from '../models/musicgenre_info';
import { API_URL } from '../app-injection-tokens';

@Injectable({
  providedIn: 'root'
})
export class MusicService {

  constructor(private http: HttpClient, @Inject(API_URL) private apiUrl: string) { }

  getListMusicGenres(): Observable<MusicGenreInfo[]> {
    return this.http.get<MusicGenreInfo[]>(`${this.apiUrl}api/music/listMusicGenres`)
  }
}
