import {Component, OnInit} from '@angular/core';
import { ApiService } from "../../modules/base/services/api.service";
import { Router } from "@angular/router";

@Component({
  selector: 'app-create-account',
  templateUrl: './create-account.component.html',
  styleUrls: ['./create-account.component.scss']
})
export class CreateAccountComponent implements OnInit {

  step = 1;
  signingUp = false;
  availablePortraits = [
    'priestess', 'sailor', 'sword'
  ];

  request = {
    vassalName: '',
    portrait: '',
    personalName: '',
    email: '',
    passphrase: '',
  };

  constructor(private api: ApiService, private router: Router) { }

  ngOnInit() {
    this.request.portrait = this.availablePortraits[Math.floor(Math.random() * this.availablePortraits.length)];
  }

  doCreateAccount()
  {
    if(this.signingUp)
      return;

    this.signingUp = true;

    this.api.setSessionId(null);
    this.api.post('accounts/create', this.request).subscribe({
      next: _ => {
        const logInRequest = {
          email: this.request.email,
          passphrase: this.request.passphrase
        };

        this.api.post<{ sessionId: string }>('accounts/logIn', logInRequest).subscribe({
          next: r => {
            this.api.setSessionId(r.data.sessionId);
            // noinspection JSIgnoredPromiseFromCall
            this.router.navigateByUrl('/home');
          }
        });
      },
      error: () => {
        this.signingUp = false;
      }
    });
  }
}
