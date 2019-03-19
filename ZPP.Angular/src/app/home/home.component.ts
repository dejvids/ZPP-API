import { Component, OnInit, Inject, Output, EventEmitter } from '@angular/core';
import { Lecture } from '../Models/Lecture';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { tap, catchError } from 'rxjs/operators';
import { Observable, of } from 'rxjs';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.css']
})
export class HomeComponent implements OnInit {

  PromotingLectures: Lecture[];
  Lectures: Lecture[];
  @Output() openedHome = new EventEmitter<boolean>();

  constructor(private http: HttpClient, @Inject('BASE_URL') private baseUrl: string) {
    http.get<Lecture[]>(baseUrl + "/api/lectures", { headers: new HttpHeaders().set('Content-Type', 'application/json; charset=utf-8') })
      .pipe(
        tap(res => console.log('ok')),
        catchError(this.handleError<any>())
      ).subscribe(res => {
        this.Lectures = res;
        if(this.Lectures.length >= 3){
          this.PromotingLectures = [this.Lectures[0], this.Lectures[1], this.Lectures[2]];
        }
      });
  }

  ngOnInit() {
    this.openedHome.emit(true);
    let l1 = new Lecture();
    l1.name = "Wykład 1";
    let l2 = new Lecture();
    l2.name = "wykład 2";
    let l3 = new Lecture();
    l3.name = "Wykład 3";

    this.PromotingLectures = [l1, l2, l3];
  }

  private handleError<T>(operation = 'operation', result?: T) {
    return (error: any): Observable<T> => {

      // TODO: send the error to remote logging infrastructure
      console.error(error); // log to console instead

      // TODO: better job of transforming error for user consumption
      if (error.status == 400) {
        console.log(`${operation} failed: ${error.error.message}`);
      }

      // Let the app keep running by returning an empty result.
      return of(result as T);
    };
  }

}
