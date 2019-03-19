import { Component, OnInit, Output, HostBinding, Input } from '@angular/core';
import { JsonWebToken } from '../Models/JsonWebToken';
import { single } from 'rxjs/operators';
import { NavbarService } from '../navbar.service';

@Component({
  selector: 'app-nav-menu',
  templateUrl: './nav-menu.component.html',
  styleUrls: ['./nav-menu.component.css']
})
export class NavMenuComponent {
  isExpanded = false;
  isSigned = false;

  constructor(private navbarService : NavbarService) {
    let token = localStorage.getItem('token');
    console.log('menu ' + token);
    if (token != null) {
      let jwt = JSON.parse(token);
      console.log(jwt.expires);
      if (jwt.expires > new Date(Date.now()).getTime()) {
        this.isSigned = true;
      }
      else {
        this.isSigned = false;
      }
    }
    else{
      this.isSigned = true;
    }
  }

  ngOnInit(){
    this.navbarService.change.subscribe(isSigned => this.isSigned = isSigned);
  }

  collapse() {
    this.isExpanded = false;
  }

  toggle() {
    this.isExpanded = !this.isExpanded;
  }
}
