import { Injectable, Inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { MusicGenreInfo } from '../models/musicgenre_info';
import { API_URL } from '../app-injection-tokens';
import { MusicInfo } from '../models/music_info';

@Injectable({
  providedIn: 'root'
})
export class MusicService {

  constructor(private http: HttpClient, @Inject(API_URL) private apiUrl: string) { }

  getListMusicGenres(): Observable<MusicGenreInfo[]> {
    return this.http.get<MusicGenreInfo[]>(`${this.apiUrl}api/music/listMusicGenres`)
  }

  getListMusicByUserId(id: number): Observable<MusicInfo[]> {
    return this.http.get<MusicInfo[]>(`${this.apiUrl}api/music/list/` + id);
  }

  addmusic(formData: FormData) {
    return this.http.post(`${this.apiUrl}api/music/AddMusic`, formData);
  }

  getFileNameByPath(path: string):string {
    var arr = path.split('\\');
    return arr[arr.length - 1];
  }

  checkFileFormat(fileName:string, regularFormat: string): boolean {
    var format = fileName.substring(fileName.indexOf('.') + 1, fileName.length)
    if (format == regularFormat)
      return true;
    return false;
    
  }
}