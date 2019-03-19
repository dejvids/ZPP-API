import { Component, OnInit, Inject, Output, EventEmitter } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { catchError, map, tap } from 'rxjs/operators';
import 'src/app/Models/User'
import { Observable, of } from 'rxjs';
import { User } from 'src/app/Models/User';
import { Router } from '@angular/router';
import { NavbarService } from '../navbar.service';
const httpOptions = {
  headers: new HttpHeaders({ 'Content-Type': 'application/json' })
};


@Component({
  selector: 'app-sign-in',
  templateUrl: './sign-in.component.html',
  styleUrls: ['./sign-in.component.css']
})
export class SignInComponent implements OnInit {
  http: HttpClient;
  baseUrl: string;
  router: Router;

  constructor(private navbarService : NavbarService, router : Router, http: HttpClient, @Inject('BASE_URL') baseUrl: string) {
    this.http = http;
    this.baseUrl = baseUrl;
    this.router = router;
  }

  errorMessage: string;
  isAlertVisible: boolean = false;

  public signIn() {
    console.log('Logowanie');
    let url = this.baseUrl + "/api/sign-in";
    console.log(url);
    let user = new User();
    user.login = "dsurys";
    user.password = "123456";
    console.log(user);
    this.http.post<SignInResult>(url, user, httpOptions)
      .pipe(
        tap(res => console.log('login ok')),
        catchError(this.handleError<SignInResult>())
      )
      .subscribe((res: SignInResult) => {
        if (res.success) {
          localStorage.setItem('token', JSON.stringify(res.token));
          this.navbarService.setSignedIn();
          this.router.navigateByUrl('/profil');
        }
      });
  }

/**
* Handle Http operation that failed.
* Let the app continue.
* @param operation - name of the operation that failed
* @param result - optional value to return as the observable result
*/
  private handleError<T>(operation = 'operation', result?: T) {
    return (error: any): Observable<T> => {

      // TODO: send the error to remote logging infrastructure
      console.error(error); // log to console instead

      // TODO: better job of transforming error for user consumption
      if (error.status == 400) {
        console.log(`${operation} failed: ${error.error.message}`);
        this.errorMessage = error.error.message;
        this.isAlertVisible = true;
      }

      // Let the app keep running by returning an empty result.
      return of(result as T);
    };
  }

  ngOnInit() {
  }

}

// class User {
//   id: number;
//   login: string;
//   Email: string;
//   name: string;
//   surname: string;
//   password: string;
// }

class SignInResult {
  success: boolean;
  message: string;
  token: JsonWebToken;
}

class JsonWebToken {
  accessToken: string;
  expires: number;
  role: string;
}
