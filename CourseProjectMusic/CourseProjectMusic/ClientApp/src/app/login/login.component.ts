import { Component, OnInit } from '@angular/core';
import { FormGroup, FormControl, Validators } from '@angular/forms';
import { AuthService } from '../services/auth.service';

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.css']
})
export class LoginComponent implements OnInit {

  form: FormGroup;
  public get isLoggedIn(): boolean {
    return this.as.isAuth();
  }

  constructor(private as: AuthService) { }

  ngOnInit(): void {
    this.form = new FormGroup({
      email: new FormControl(null, [Validators.required, Validators.email]),
      password: new FormControl(null, [Validators.required, Validators.minLength(6)])
    });
  }

  login() {
    this.as.login(this.form.value.email, this.form.value.password).subscribe(res => {
      alert("auth:ok!");
    }, error => {
        alert('неверный логин или пароль');
    });
  }

  logout() {
    this.as.logout();
  }

}
