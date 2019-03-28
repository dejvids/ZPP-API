import { Component, OnInit, Inject } from '@angular/core';
import { Router } from '@angular/router';
import { logging } from 'protractor';
import { HttpClient, HttpHeaders  } from '@angular/common/http';
import { User } from '../Models/User';
import { tap, catchError } from 'rxjs/operators';
import { Observable, of } from 'rxjs';
const httpOptions = {
  headers: new HttpHeaders({ 'Content-Type': 'application/json' })
};

@Component({
  selector: 'app-sign-up',
  templateUrl: './sign-up.component.html',
  styleUrls: ['./sign-up.component.css']
})
export class SignUpComponent implements OnInit {

  router: Router;

  successfullyRegistered: boolean;
  isAlertVisible: boolean;
  message: string;
  login: string ='';
  email: string ='';
  name: string ='';
  surname: string ='';
  password: string='';
  repeatedPassword: string='';
  http: HttpClient;
  baseUrl: string;

  constructor(router: Router, http: HttpClient, @Inject('BASE_URL') baseUrl: string) {
    this.router = router;
    this.http = http;
    this.baseUrl = baseUrl;
  }

  ngOnInit() {
    let token = localStorage.getItem('token');
    console.log('menu ' + token);
    if (token != null) {
      let jwt = JSON.parse(token);
      console.log(jwt.expires);
      if (jwt.expires > new Date().getTime()) {
        this.router.navigateByUrl('/profil');
      }
    }
  }

  public signUpAsync() {
    console.log('Rejestracja');
    this.isAlertVisible = false;
    if(!this.validateModel()){
      this.isAlertVisible = true;
    }

    let user = new User();
    user.login = this.login;
    user.email = this.email;
    user.password = this.password;
    user.name = this.name;
    user.surname = this.surname;
    
    let url = this.baseUrl + "/api/sign-up";
    this.http.post<SignUpResult>(url, user, httpOptions)
      .pipe(
        tap(res => console.log(' ok')),
        catchError(this.handleError<SignUpResult>())
      )
      .subscribe((res: SignUpResult) => {
        if (res.success) {
          this.message= res.message;
          this.successfullyRegistered = true;
        }
      });
  }

  private validateModel():boolean {
    this.message = '';
    if (this.login.length == 0 || this.email.length == 0 || this.password.length == 0) {
      this.message = "Wypełnij wszystkie wymagane pola formularza";
      return false;
    }
    else if (this.password !== this.repeatedPassword) {
      this.message = "Hasło i powtórzone hasło muszą być takie same";
      return false;
    }
    else if (this.password.length < 6){
      this.message = "Hasło musi zawierać co najmniej 6 znaków";
      return false;
    }
    else if(!this.login.match('^[A-Z a-z]+')){
      this.message = "Login musi rozpoczynać się od litery";
      return false;
    }
    else if(!this.password.match('[0-9]+')){
      this.message = "Hasło musi zawierać co najmniej jedną cyfrę";
      return false;
    }
    
    else if(!this.password.match('[A-Z a-z]+')){
      this.message = "Hasło musi zawierać co najmniej jedną literę";
      return false;
    }
    var re = new RegExp(/^(([^<>()\[\]\\.,;:\s@"]+(\.[^<>()\[\]\\.,;:\s@"]+)*)|(".+"))@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\])|(([a-zA-Z\-0-9]+\.)+[a-zA-Z]{2,}))$/);
    if(!re.test(this.email.toLowerCase())){
      this.message = "Niepoprawny adres e-mail";
      return false;
    }
    return true;
  }

  private handleError<T>(operation = 'operation', result?: T) {
    return (error: any): Observable<T> => {

      // TODO: send the error to remote logging infrastructure
      console.error(error); // log to console instead

      // TODO: better job of transforming error for user consumption
      if (error.status == 400) {
        console.log(`${operation} failed: ${error.error.message}`);
        this.message = error.error.message;
        this.isAlertVisible = true;
      }

      // Let the app keep running by returning an empty result.
      return of(result as T);
    };
  }
}

class SignUpResult
{
  message : string;
  success : boolean;
}