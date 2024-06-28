import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { NewsComponent } from './page/news/news.component';
import { NewsRoutingModule } from "./news-routing.module";
import { GameCommonModule } from "../game-common/game-common.module";
import { BaseModule } from "../base/base.module";
import { MarkdownModule } from "ngx-markdown";



@NgModule({
  declarations: [
    NewsComponent
  ],
  imports: [
    CommonModule,
    NewsRoutingModule,
    GameCommonModule,
    BaseModule,
    MarkdownModule
  ]
})
export class NewsModule { }
