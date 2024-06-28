import { Component, OnInit } from '@angular/core';
import {ApiService} from "../../../base/services/api.service";
import {MessagesService} from "../../../base/services/messages.service";

@Component({
  selector: 'app-email-address',
  templateUrl: './email-address.component.html',
  styleUrls: ['./email-address.component.scss']
})
export class EmailAddressComponent implements OnInit {

  newEmailAddress = '';

  accountInfo: AccountInfoDto|null = null;

  working = false;

  constructor(private api: ApiService, private messages: MessagesService) { }

  ngOnInit(): void {
    this.api.get<AccountInfoDto>('accounts/info').subscribe({
      next: r => {
        this.accountInfo = r.data;
      }
    });
  }

  doChangeEmailAddress()
  {
    if(this.working)
      return;

    this.working = true;

    const data = {
      email: this.newEmailAddress,
    };

    this.api.post<{ email: string }>('accounts/changeEmail', data).subscribe({
      next: r => {
        this.working = false;

        if(this.accountInfo) this.accountInfo.emailAddress = r.data.email;

        this.newEmailAddress = '';

        this.messages.add({
          type: 'Success',
          text: 'Your email address has been updated!'
        });
      },
      error: () => {
        this.working = false;
      }
    });
  }

}

interface AccountInfoDto
{
  id: string;
  signUpDate: string;
  emailAddress: string;
}
