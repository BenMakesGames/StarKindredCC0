import { Component, OnInit } from '@angular/core';
import {Router} from "@angular/router";

@Component({
  template: ''
})
export class NotFoundComponent implements OnInit {

  constructor(private router: Router) { }

  ngOnInit(): void {
    // noinspection JSIgnoredPromiseFromCall
    this.router.navigateByUrl('/');
  }

}
