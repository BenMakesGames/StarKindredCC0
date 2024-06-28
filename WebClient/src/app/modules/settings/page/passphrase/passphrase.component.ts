import { Component, OnInit } from '@angular/core';
import {MessagesService} from "../../../base/services/messages.service";
import {ApiService} from "../../../base/services/api.service";

@Component({
  selector: 'app-passphrase',
  templateUrl: './passphrase.component.html',
  styleUrls: ['./passphrase.component.scss']
})
export class PassphraseComponent implements OnInit {

  working = false;

  newPassphrase = '';
  newPassphrase2 = '';

  constructor(private messages: MessagesService, private api: ApiService) { }

  ngOnInit(): void {
  }

  doChangePassphrase()
  {
    if(this.working)
      return;

    if(this.newPassphrase !== this.newPassphrase2)
    {
      this.messages.add({
        type: 'Error',
        text: 'Passphrases do not match.'
      });

      return;
    }

    this.working = true;

    const data = {
      newPassphrase: this.newPassphrase,
    }

    this.api.post('accounts/changePassphrase', data).subscribe({
      next: () => {
        this.working = false;
        this.newPassphrase = '';
        this.newPassphrase2 = '';

        this.messages.add({
          type: 'Success',
          text: 'Your passphrase has been updated!'
        });
      },
      error: () => {
        this.working = false;
      }
    });
  }

}
