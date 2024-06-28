import {Component, Input, OnDestroy, OnInit} from '@angular/core';
import {ApiService} from "../../services/api.service";
import {NavigationStart, Router} from "@angular/router";
import {YesOrNoDialog} from "../../dialogs/yes-or-no/yes-or-no.dialog";
import {MatDialog} from "@angular/material/dialog";
import {of, Subscription} from "rxjs";
import {NewNewsService} from "../../services/new-news.service";
import {NavMenuEvent, NavMenuService} from "../../services/nav-menu.service";

@Component({
  selector: 'app-nav',
  templateUrl: './nav.component.html',
  styleUrls: ['./nav.component.scss']
})
export class NavComponent implements OnInit, OnDestroy {

  open = false;
  loggedIn = false;
  routerSubscription = Subscription.EMPTY;
  sessionIdSubscription = Subscription.EMPTY;
  newNewsSubscription = Subscription.EMPTY;
  navMenuServiceSubscription = Subscription.EMPTY;
  unreadNews = false;
  hidden = false;

  constructor(
    private api: ApiService, private router: Router, private matDialog: MatDialog, private newNews: NewNewsService,
    private navMenuService: NavMenuService
  ) {
  }

  ngOnInit() {
    this.newNewsSubscription = this.newNews.unreadNews.subscribe(unread => this.unreadNews = unread);

    this.navMenuServiceSubscription = this.navMenuService.events.subscribe({
      next: (e: NavMenuEvent) => {
        this.hidden = e == NavMenuEvent.Hide;
        if(this.hidden)
          this.open = false;
      }
    });

    this.sessionIdSubscription = this.api.sessionId.subscribe({
      next: sessionId => {
        this.loggedIn = !!sessionId;
      }
    });

    this.routerSubscription = this.router.events.subscribe({
      next: e => {
        if(e instanceof NavigationStart)
        {
          this.hidden = false;
          this.open = false;
        }
      }
    });
  }

  ngOnDestroy() {
    this.sessionIdSubscription.unsubscribe();
    this.routerSubscription.unsubscribe();
    this.newNewsSubscription.unsubscribe();
    this.navMenuServiceSubscription.unsubscribe();
  }

  doToggleNav()
  {
    this.open = !this.open;
  }

  doLogOut()
  {
    YesOrNoDialog.open(this.matDialog, () => this.reallyLogOut(), 'Log Out').afterClosed().subscribe({
      next: r => {
        if(r)
          this.router.navigateByUrl('/');
      }
    })
  }

  private reallyLogOut()
  {
    this.api.post('accounts/logOut').subscribe();
    this.api.setSessionId(null);
    return of(true);
  }

}
