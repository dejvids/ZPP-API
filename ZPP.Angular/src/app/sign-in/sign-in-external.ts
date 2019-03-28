import { Component, OnInit, Inject, Output, EventEmitter } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { catchError, map, tap } from 'rxjs/operators';
import 'src/app/Models/User'
import { Observable, of } from 'rxjs';
import { User } from 'src/app/Models/User';
import { Router, ActivatedRoute, Params } from '@angular/router';
import { NavbarService } from '../navbar.service';
import { JsonWebToken } from '../Models/JsonWebToken';
const httpOptions = {
    headers: new HttpHeaders({ 'Content-Type': 'application/json' })
};


@Component({
    selector: 'app-sign-in-external',
    template: ''
})
export class SignInExternal implements OnInit {

    router : Router;
    activatedRoute : ActivatedRoute;
    constructor(router : Router, activatedRoute : ActivatedRoute) {
        this.router = router;
        this.activatedRoute = activatedRoute;
    }
    ngOnInit() {
        console.log('Logowanie zewnętrzne');
        this.activatedRoute.queryParams.subscribe(params=>
            {
                let token = new JsonWebToken();
                token.accessToken = params['token'];
                token.expires = params['expires'];
                token.role = params['role'];              
                localStorage.setItem('token', JSON.stringify(token));
                console.log(token);
                location.assign('/profil')
            });
    }

}