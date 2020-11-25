import { Component, OnInit } from '@angular/core';
import { MusicGenreInfo } from '../models/musicgenre_info';
import { MusicService } from '../services/music.service';
import { FormGroup, FormControl } from '@angular/forms';
import { FilteredMusicList } from '../models/filtered_music';

@Component({
  selector: 'app-search-music',
  templateUrl: './search-music.component.html',
  styleUrls: ['./search-music.component.css']
})
export class SearchMusicComponent implements OnInit {

  musicGenres: MusicGenreInfo[] = [];
  form: FormGroup;
  selectedOption = '0';
  filteredList: FilteredMusicList = new FilteredMusicList();

  constructor(private musicService: MusicService) { }

  ngOnInit() {
    this.musicService.getListMusicGenres().subscribe(data => {
      this.musicGenres = data
    }, error => {
      alert(error)
    });

    this.form = new FormGroup({
      musicName: new FormControl(""),
      musicGenreId: new FormControl("0")
    })
  }

  search() {
    this.filteredList.musicName = this.form.value.musicName;
    this.filteredList.genreId = this.form.value.musicGenreId;
    this.musicService.getFilteredMusicList(this.filteredList).subscribe(result => {
      console.log(result)
    }, error => {
        alert('Статусный код '+error.status)
    })
  }
}
