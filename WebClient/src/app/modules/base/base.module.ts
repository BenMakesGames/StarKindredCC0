import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import {HttpClientModule} from "@angular/common/http";
import { MessagesComponent } from './components/messages/messages.component';
import { MessageComponent } from './components/message/message.component';
import { LoadingComponent } from './components/loading/loading.component';
import { DialogComponent } from './components/dialog/dialog.component';
import {DescribeIntervalPipe} from "./pipes/describe-interval.pipe";
import { PaginationComponent } from './components/pagination/pagination.component';
import {RouterModule} from "@angular/router";
import {YesOrNoDialog} from "./dialogs/yes-or-no/yes-or-no.dialog";
import {NavComponent} from "./components/nav/nav.component";
import {MarkdownModule} from "ngx-markdown";


@NgModule({
  declarations: [
    MessagesComponent,
    MessageComponent,
    LoadingComponent,
    DialogComponent,
    DescribeIntervalPipe,
    PaginationComponent,
    YesOrNoDialog,
    NavComponent,
  ],
  exports: [
    MessagesComponent,
    LoadingComponent,
    DialogComponent,
    DescribeIntervalPipe,
    PaginationComponent,
    YesOrNoDialog,
    NavComponent,
  ],
  imports: [
    CommonModule,
    HttpClientModule,
    RouterModule,
    MarkdownModule,
  ]
})
export class BaseModule { }
