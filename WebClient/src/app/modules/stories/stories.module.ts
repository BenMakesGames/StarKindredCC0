import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ListComponent } from './page/list/list.component';
import {StoriesRoutingModule} from "./stories-routing.module";
import {GameCommonModule} from "../game-common/game-common.module";
import {BaseModule} from "../base/base.module";
import {ViewComponent} from "./page/view/view.component";
import {MarkdownModule} from "ngx-markdown";
import { MonthNamePipe } from './pipe/month-name.pipe';
import {DoStoryDialog} from "./dialogs/do-story/do-story.dialog";
import {ShowCompletedStoryDialog} from "./dialogs/show-completed-story/show-completed-story.dialog";



@NgModule({
  declarations: [
    ListComponent,
    ViewComponent,
    MonthNamePipe,
    DoStoryDialog,
    ShowCompletedStoryDialog,
  ],
  imports: [
    CommonModule,
    StoriesRoutingModule,
    GameCommonModule,
    BaseModule,
    MarkdownModule,
  ]
})
export class StoriesModule { }
