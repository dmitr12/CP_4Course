import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, FormControl } from '@angular/forms';

@Component({
  selector: 'app-register',
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.css']
})
export class RegisterComponent implements OnInit {

  form: FormGroup;

  constructor(private formBuilder: FormBuilder) { }

  ngOnInit() {
    //this.form = this.formBuilder.group({
    //  email: ['ss'],
    //  login: [''],
    //  password: [''],
    //  confirmpassword: ['']
    //}, {
    //    validator: MustMatch('password:', 'confirmpassword')
    //});
  }

}
