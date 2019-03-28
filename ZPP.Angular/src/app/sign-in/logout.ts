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
    selector: 'app-log-out',
    template:''
})
export class LogoutComponent implements OnInit {

    router : Router;
    constructor(router : Router) {
        this.router = router;
    }
    ngOnInit() {

        localStorage.removeItem('token');
        location.assign('/');
    }
}