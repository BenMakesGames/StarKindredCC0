import { Injectable } from '@angular/core';
import {ApiService} from "./api.service";
import {BehaviorSubject, interval, mergeMap, startWith, Subscription} from "rxjs";

@Injectable({
  providedIn: 'root'
})
export class NewNewsService {

  unreadNews = new BehaviorSubject<boolean>(false);

  sessionIdSubscription = Subscription.EMPTY;
  checkForNewsSubscription = Subscription.EMPTY;

  constructor(private api: ApiService) {
    this.sessionIdSubscription = this.api.sessionId.subscribe({
      next: sessionId => {
        if(sessionId)
          this.restartNewsChecking();
        else
          this.checkForNewsSubscription.unsubscribe();
      }
    });
  }

  restartNewsChecking()
  {
    this.checkForNewsSubscription.unsubscribe();
    this.checkForNewsSubscription = interval(1000 * 60 * 5)
      .pipe(
        startWith(0),
        mergeMap(() => this.api.get<{ unreadAnnouncements: boolean }>('announcements/anyUnread'))
      )
      .subscribe(r => {
        this.unreadNews.next(r.data.unreadAnnouncements);
      })
    ;
  }


}
