import { Component, OnInit } from '@angular/core';
import {ActivatedRoute, Router} from "@angular/router";
import {MessagesService} from "../../modules/base/services/messages.service";
import {ApiService} from "../../modules/base/services/api.service";

@Component({
  templateUrl: './magic-login.component.html',
  styleUrls: ['./magic-login.component.scss']
})
export class MagicLoginComponent implements OnInit {

  constructor(
    private activatedRoute: ActivatedRoute,
    private messages: MessagesService,
    private router: Router,
    private api: ApiService
  ) { }

  ngOnInit(): void {
    this.activatedRoute.queryParams.subscribe(params => {
      const result = (params['result'] ?? 'invalid').trim();

      if(result == 'invalid' || result == '')
      {
        this.messages.add({
          type: 'Error',
          text: 'That magic link does not exist.'
        });
      }
      else if(result == 'expired')
      {
        this.messages.add({
          type: 'Error',
          text: 'That magic link expired.'
        });
      }
      else
      {
        this.api.setSessionId(result);
      }

      this.router.navigate(['/']);
    });
  }

}
