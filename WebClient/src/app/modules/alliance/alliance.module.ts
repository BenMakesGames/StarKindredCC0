import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { AllianceComponent } from "./page/alliance/alliance.component";
import { CreateAllianceComponent } from "./page/create-alliance/create-alliance.component";
import { AllianceMemberDialog } from "./dialogs/alliance-member/alliance-member.dialog";
import { AllianceRoutingModule } from "./alliance-routing.module";
import { GameCommonModule } from "../game-common/game-common.module";
import { BaseModule } from "../base/base.module";
import { FormsModule } from "@angular/forms";
import {RecruitDialog} from "./dialogs/recruit/recruit.dialog";
import { AllianceLogsComponent } from './page/alliance-logs/alliance-logs.component';
import { IndexComponent } from './page/index/index.component';
import { ViewComponent } from './page/view/view.component';
import { MemberCardComponent } from './components/member-card/member-card.component';
import { RanksComponent } from './page/ranks/ranks.component';
import { CreateRankDialog } from './dialogs/create-rank/create-rank.dialog';
import {RenameRankDialog} from "./dialogs/rename-rank/rename-rank.dialog";
import {MarkdownModule} from "ngx-markdown";
import {EnterInviteCodeDialog} from "./dialogs/enter-invite-code/enter-invite-code.dialog";
import {ChangeMemberTitleDialog} from "./dialogs/change-member-title/change-member-title.dialog";
import {MatDialogModule} from "@angular/material/dialog";

@NgModule({
  declarations: [
    AllianceComponent,
    CreateAllianceComponent,
    AllianceMemberDialog,
    RecruitDialog,
    AllianceLogsComponent,
    IndexComponent,
    ViewComponent,
    MemberCardComponent,
    RanksComponent,
    CreateRankDialog,
    RenameRankDialog,
    EnterInviteCodeDialog,
    ChangeMemberTitleDialog,
  ],
    imports: [
        CommonModule,
        AllianceRoutingModule,
        GameCommonModule,
        BaseModule,
        FormsModule,
        MarkdownModule,
        MatDialogModule,
    ]
})
export class AllianceModule { }
