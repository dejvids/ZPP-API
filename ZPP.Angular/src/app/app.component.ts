import { Component } from '@angular/core';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent {
  title = 'ZPP Angular';

  Signed: boolean =false;

  public setSigned(isSigned : boolean) {
     this.Signed = true;
   }
}
