import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';

@Component({
  selector: 'app-sign-up',
  templateUrl: './sign-up.component.html',
  styleUrls: ['./sign-up.component.css']
})
export class SignUpComponent implements OnInit {

  router : Router;
  constructor(router : Router) {
    this.router = router;
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

}
