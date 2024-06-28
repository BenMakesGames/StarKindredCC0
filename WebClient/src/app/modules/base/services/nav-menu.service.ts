import { Injectable } from '@angular/core';
import { Subject} from "rxjs";

@Injectable({
  providedIn: 'root'
})
export class NavMenuService {

  events = new Subject<NavMenuEvent>();

  constructor() { }

  hide()
  {
    this.events.next(NavMenuEvent.Hide);
  }

  show()
  {
    this.events.next(NavMenuEvent.Show);
  }
}

export enum NavMenuEvent {
  Hide,
  Show
}
