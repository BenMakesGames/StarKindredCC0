import {Component, OnInit} from '@angular/core';
import {ApiService} from "../../modules/base/services/api.service";

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.scss']
})
export class HomeComponent implements OnInit {

  loggedInAs: string|null = null;
  loggingIn = false;

  constructor(private api: ApiService) { }

  ngOnInit() {
    if(this.api.sessionId.value)
    {
      this.loggingIn = true;

      this.api.get<{ name: string }>('accounts/info').subscribe({
        next: r => {
          this.loggedInAs = r.data.name;
          this.loggingIn = false;
        },
        error: () => {
          this.loggingIn = false;
        }
      });
    }
  }

  doLogOut()
  {
    if(this.loggingIn)
      return;

    this.loggingIn = true;

    this.api.setSessionId(null);

    this.api.post('accounts/logOut').subscribe({
      next: () => {
        this.loggingIn = false;
        this.loggedInAs = null;
      },
      error: () => {
        this.loggingIn = false;
        this.loggedInAs = null;
      }
    });
  }
}
