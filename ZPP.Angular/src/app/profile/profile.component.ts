import { Component, OnInit, Inject } from '@angular/core';
import { User } from '../Models/User';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { tap, catchError } from 'rxjs/operators';
import { Observable, of } from 'rxjs';
import { Router } from '@angular/router';
import { JsonWebToken } from '../Models/JsonWebToken';


@Component({
  selector: 'app-profile',
  templateUrl: './profile.component.html',
  styleUrls: ['./profile.component.css']
})
export class ProfileComponent implements OnInit {

  user: User;
  roter: Router;
  http: HttpClient;
  constructor(router: Router, http: HttpClient, @Inject('BASE_URL') baseUrl: string) {
    this.roter = router;
    this.http = http;
    this.user = new User();

    let token = localStorage.getItem('token');
    if (token == null) {
      this.roter.navigateByUrl('/sign-in');
      return;
    }

    let tokenObj = JSON.parse(token);
    let jwt = tokenObj.accessToken;
    console.log(jwt);
    let httpHeaders =
      new HttpHeaders()
        .set('Content-Type', 'application/json; charset=utf-8')
        .set('Authorization', 'Bearer ' + jwt);

    console.log(httpHeaders.getAll('Authorization'));

    http.get<User>(baseUrl + "/api/me", { headers: httpHeaders })
      .pipe(
        tap(res => console.log('ok')),
        catchError(this.handleError<any>())
      ).subscribe(res => {
        console.log(res);
        this.user = res;
        // this.user = new User();
        // this.user.login = res.login;
        // this.user.name = res.name;
        // this.user.surname = res.surname;
        // this.user.email = res.email;
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
      }

      if (error.status == 401) {
        this.roter.navigateByUrl("/sign-in");
        return;
      }

      // Let the app keep running by returning an empty result.
      return of(result as T);
    };
  }

  ngOnInit() {
  }

}
