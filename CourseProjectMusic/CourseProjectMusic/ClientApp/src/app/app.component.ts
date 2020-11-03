import { Component } from '@angular/core';
import { AuthService } from './services/auth.service';
import { error } from '@angular/compiler/src/util';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html'
})
export class AppComponent {
  public get isLoggedIn(): boolean {
    return this.as.isAuth();
  }

  constructor(private as: AuthService) { }

  login(username: string, passsword: string) {
    this.as.login(username, passsword).subscribe(res => {
      alert("auth:ok!");
    }, error => {
      alert('wrong login or password');
    });
  }

  logout() {
    this.as.logout();
  }
}
