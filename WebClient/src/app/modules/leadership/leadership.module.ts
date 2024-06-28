import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { LeadershipComponent } from './page/leadership/leadership.component';
import {LeadershipRoutingModule} from "./leadership-routing.module";
import {GameCommonModule} from "../game-common/game-common.module";
import {BaseModule} from "../base/base.module";
import { LeadersComponent } from './components/leaders/leaders.component';
import {AssignVassalToPositionDialog} from "./dialogs/assign-vassal-to-position/assign-vassal-to-position.dialog";
import {ResearchDialog} from "./dialogs/research/research.dialog";
import {FormsModule} from "@angular/forms";
import { ResearchTimeComponent } from './components/research-time/research-time.component';


@NgModule({
  declarations: [
    LeadershipComponent,
    LeadersComponent,
    AssignVassalToPositionDialog,
    ResearchDialog,
    ResearchTimeComponent,
  ],
  imports: [
    CommonModule,
    LeadershipRoutingModule,
    GameCommonModule,
    BaseModule,
    FormsModule,
  ]
})
export class LeadershipModule { }
