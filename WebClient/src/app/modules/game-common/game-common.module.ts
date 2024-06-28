import {NgModule} from '@angular/core';
import {CommonModule} from '@angular/common';
import {VassalPortraitComponent} from './components/vassal-portrait/vassal-portrait.component';
import {RouterModule} from "@angular/router";
import {ResourceComponent} from "./components/resource/resource.component";
import {ResourcesComponent} from './components/resources/resources.component';
import {VassalMedallionComponent} from './components/vassal-medallion/vassal-medallion.component';
import {LevelBadgeComponent} from './components/level-badge/level-badge.component';
import {ProgressBarComponent} from './components/progress-bar/progress-bar.component';
import {HelpDialog} from "./dialogs/help/help.dialog";
import { StarsComponent } from './components/stars/stars.component';
import { TimeRemainingsComponent } from './components/time-remainings/time-remainings.component';
import { HelpButtonComponent } from './components/help-button/help-button.component';
import {MarkdownModule} from "ngx-markdown";
import { BaseModule } from "../base/base.module";
import { CardRibbonComponent } from './components/card-ribbon/card-ribbon.component';
import {OkDialog} from "./dialogs/ok/ok.dialog";
import {StatusEffectImagePipe} from "./pipes/status-effect-image.pipe";
import {HelpKeyPipe} from "./pipes/help-key.pipe";
import {EmptyVassalMedallionComponent} from "./components/empty-vassal-medallion/empty-vassal-medallion.component";
import { YesNoToggleComponent } from './components/yes-no-toggle/yes-no-toggle.component';
import { TagComponent } from './components/tag/tag.component';
import { GenericCardComponent } from './components/generic-card/generic-card.component';
import {ChangeAppearanceDialog} from "./dialogs/change-appearance/change-appearance.dialog";
import {FormsModule} from "@angular/forms";
import { VassalFiltersComponent } from './components/vassal-filters/vassal-filters.component';
import {StatusEffectTitlePipe} from "./pipes/status-effect-title.pipe";
import {ChangeRibbonDialog} from "./dialogs/change-ribbon/change-ribbon.dialog";
import {HslToCssPipe} from "./pipes/hsl-to-css.pipe";
import {DescribeYieldComponent} from "./components/describe-yield/describe-yield.component";
import {WeaponMedallionComponent} from "./components/weapon-medallion/weapon-medallion.component";
import {MissionTitlePipe} from "./pipes/mission-title.pipe";
import { MissionPinComponent } from './components/mission-pin/mission-pin.component';
import {AssembleATeamComponent} from "./components/assemble-a-team/assemble-a-team.component";
import {SelectVassalsComponent} from "./components/select-vassals/select-vassals.component";
import {NatureTitlePipe} from "./pipes/nature-title.pipe";
import {SignTitlePipe} from "./pipes/sign-title.pipe";
import { VassalMedallionListComponent } from './components/vassal-medallion-list/vassal-medallion-list.component';
import { DescribeVassalRangeComponent } from './components/describe-vassal-range/describe-vassal-range.component';
import {RollingResourceComponent} from "./components/rolling-resource/rolling-resource.component";
import {CountUpModule} from "ngx-countup";
import { LogEntryComponent } from './components/log-entry/log-entry.component';
import {LogTagColorPipe} from "./pipes/log-tag-color.pipe";
import {LogTagTitlePipe} from "./pipes/log-tag-title.pipe";

@NgModule({
  declarations: [
    VassalPortraitComponent,
    ResourceComponent,
    ResourcesComponent,
    VassalMedallionComponent,
    LevelBadgeComponent,
    ProgressBarComponent,
    HelpDialog,
    StarsComponent,
    TimeRemainingsComponent,
    HelpButtonComponent,
    CardRibbonComponent,
    OkDialog,
    StatusEffectImagePipe,
    HelpKeyPipe,
    EmptyVassalMedallionComponent,
    YesNoToggleComponent,
    TagComponent,
    GenericCardComponent,
    ChangeAppearanceDialog,
    VassalFiltersComponent,
    StatusEffectTitlePipe,
    ChangeRibbonDialog,
    HslToCssPipe,
    DescribeYieldComponent,
    WeaponMedallionComponent,
    MissionTitlePipe,
    MissionPinComponent,
    AssembleATeamComponent,
    SelectVassalsComponent,
    NatureTitlePipe,
    SignTitlePipe,
    VassalMedallionListComponent,
    DescribeVassalRangeComponent,
    RollingResourceComponent,
    LogEntryComponent,
    LogTagColorPipe,
    LogTagTitlePipe
  ],
    exports: [
        VassalPortraitComponent,
        ResourceComponent,
        ResourcesComponent,
        VassalMedallionComponent,
        ProgressBarComponent,
        HelpDialog,
        TimeRemainingsComponent,
        HelpButtonComponent,
        CardRibbonComponent,
        StatusEffectImagePipe,
        HelpKeyPipe,
        EmptyVassalMedallionComponent,
        YesNoToggleComponent,
        TagComponent,
        GenericCardComponent,
        StarsComponent,
        ChangeAppearanceDialog,
        VassalFiltersComponent,
        StatusEffectTitlePipe,
        HslToCssPipe,
        DescribeYieldComponent,
        WeaponMedallionComponent,
        MissionTitlePipe,
        MissionPinComponent,
        AssembleATeamComponent,
        SelectVassalsComponent,
        NatureTitlePipe,
        SignTitlePipe,
        VassalMedallionListComponent,
        DescribeVassalRangeComponent,
        RollingResourceComponent,
        LogEntryComponent,
      LogTagColorPipe,
      LogTagTitlePipe,
    ],
  imports: [
    CommonModule,
    RouterModule,
    MarkdownModule.forChild(),
    BaseModule,
    FormsModule,
    CountUpModule
  ]
})
export class GameCommonModule {
}
