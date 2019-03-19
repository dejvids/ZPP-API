import { Injectable, Output, EventEmitter } from '@angular/core';

@Injectable({
  providedIn: 'root'
})
export class NavbarService {

  isSigned = false;

  @Output() change: EventEmitter<boolean> = new EventEmitter<boolean>();

  setSignedIn(){
    this.isSigned = true;
    this.change.emit(this.isSigned);
  }

  constructor() { }
}
