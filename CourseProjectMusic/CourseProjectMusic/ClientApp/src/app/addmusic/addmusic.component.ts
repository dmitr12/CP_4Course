import { Component, OnInit, Inject } from '@angular/core';
import { MusicGenreInfo } from '../models/musicgenre_info';
import { MusicService } from '../services/music.service';
import { FormGroup, FormControl, Validators } from '@angular/forms';
import { HttpClient } from '@angular/common/http';
import { API_URL } from '../app-injection-tokens';

@Component({
  selector: 'app-addmusic',
  templateUrl: './addmusic.component.html',
  styleUrls: ['./addmusic.component.css']
})
export class AddmusicComponent implements OnInit {

  musicGenres: MusicGenreInfo[] = [];
  musicToUpload: FormData;
  form: FormGroup;

  files: string[] = [];
  fileToUpload: FormData;

  constructor(private musicService: MusicService, private http: HttpClient, @Inject(API_URL) private apiUrl: string) { }

  ngOnInit() {

    this.form = new FormGroup({
      musicName: new FormControl(null, [Validators.required, Validators.maxLength(100)]),
      musicFileName: new FormControl(null, [Validators.required]),
      //musicImageName: new FormControl(null),
      musicGenreId: new FormControl(null, [Validators.required])
    })

    this.musicService.getListMusicGenres().subscribe(data => {
      this.musicGenres = data
    }, error => {
      alert(error)
    });
  }

  changeFile(files: any) {
    let formData: FormData = new FormData();
    formData.append("asset", files[0], files[0].name);
    this.fileToUpload = formData;
    //this.onUploadFiles();
  }

  add() {
    let fileImageName;
    let arrMusic = this.form.value.musicFileName.split('\\');
    let fileMusicName = arrMusic[arrMusic.length - 1];
    //if (this.form.value.musicImageName===null)
    //  fileImageName = '';
    //else {
    //  let arrImage = this.form.value.musicImageName.split('\\');
    //  fileImageName = arrImage[arrImage.length - 1];
    //}
    let musicFormat = fileMusicName.substring(fileMusicName.indexOf('.') + 1, fileMusicName.length)
    if (musicFormat != 'mp3') {
      alert('Выбран неверный формат аудозаписи')
      return;
    }
    //if (fileImageName) {
    //  let imageFormat = fileImageName.substring(fileImageName.indexOf('.') + 1, fileImageName.length)
    //  if (imageFormat != 'png' && imageFormat!='jpg') {
    //    alert('Выбран неверный формат изображения')
    //    return;
    //  }
    //}
    return this.http.post(`${this.apiUrl}api/music/AddMusic?musicName=` + this.form.value.musicName + '&musicGenreId=' + this.form.value.musicGenreId, this.fileToUpload)
      .subscribe((response: any) => {
        //this.fileUpoadInitiated = false;
        //this.fileUpload = '';
        //if (response == true) {
        //  this.showBlobs();
        //}
        //else {
        //  alert('Error occured!');
        //  this.fileUpoadInitiated = false;
          alert(response['name'])
        },
          err => console.log(err),
      );
  }

}
