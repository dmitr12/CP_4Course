import { Injectable } from '@angular/core';
import * as moment from 'moment';

@Injectable({
  providedIn: 'root'
})
export class AudioService {

  constructor() { }

  audioObj = new Audio();

  currentTime = '00:00';
  duration = '00:00';
  delay;
  isPlaying = false;
  audioName = "";
  currentAudioUrl: string = null;

  openFile(name, url) {
    this.currentAudioUrl = url;
    this.audioName = name;
    this.audioObj.src = url;
    this.audioObj.load();
    this.play();
    this.updateProgress();
  }

  play() {
    this.audioObj.play();
    if (this.currentAudioUrl)
      this.isPlaying = true;
  }

  pause() {
    this.audioObj.pause();
    if (this.currentAudioUrl)
      this.isPlaying = false;
  }

  stop() {
    this.audioObj.pause();
    this.audioObj.currentTime = 0;
  }

  timeFormat(time, format = "mm:ss") {
    const momentTime = time * 1000;
    return moment.utc(momentTime).format(format);
  }

  mouseDown() {
    clearTimeout(this.delay);
    this.audioObj.pause();
  }

  mouseUp() {
    this.audioObj.play();
    this.updateProgress();
  }

  updateProgress() {
    this.currentTime = this.timeFormat(this.audioObj.currentTime);
    this.duration = this.timeFormat(this.audioObj.duration);
    this.delay = setTimeout(() => {
      this.updateProgress();
    }, 1000);
  }

  mute() {
    this.audioObj.volume = 0;
  }
}
