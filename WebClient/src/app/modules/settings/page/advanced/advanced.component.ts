import { Component, OnInit } from '@angular/core';
import {ApiService} from "../../../base/services/api.service";

@Component({
  selector: 'app-advanced',
  templateUrl: './advanced.component.html',
  styleUrls: ['./advanced.component.scss']
})
export class AdvancedComponent implements OnInit {

  accountInfo: AccountInfoDto|null = null;

  constructor(private api: ApiService) { }

  ngOnInit(): void {
    this.api.get<AccountInfoDto>('accounts/info').subscribe({
      next: r => {
        this.accountInfo = r.data;
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
