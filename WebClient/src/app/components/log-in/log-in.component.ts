import { Component, OnInit } from '@angular/core';
import {ApiService} from "../../modules/base/services/api.service";
import {Router} from "@angular/router";

@Component({
  selector: 'app-log-in',
  templateUrl: './log-in.component.html',
  styleUrls: ['./log-in.component.scss']
})
export class LogInComponent implements OnInit {

  panelIndex = 0;

  loggingIn = false;
  logInRequest = { email: '', passphrase: '' };

  constructor(private api: ApiService, private router: Router) { }

  ngOnInit(): void {
  }

  doLogIn()
  {
    if(this.loggingIn)
      return;

    this.loggingIn = true;

    this.api.setSessionId(null);
    this.api.post<{ sessionId: string }>('accounts/logIn', this.logInRequest).subscribe({
      next: r => {
        this.api.setSessionId(r.data.sessionId);
        this.router.navigateByUrl('/home');
      },
      error: () => {
        this.loggingIn = false;
      }
    });
  }

  doEmailMagicLink()
  {
    if(this.loggingIn)
      return;

    this.loggingIn = true;

    this.api.post('accounts/emailMagicLink', { email: this.logInRequest.email }).subscribe({
      next: _ => {
        this.loggingIn = false;
        this.panelIndex = 4;
      },
      error: () => {
        this.loggingIn = false;
      }
    });
  }

}
