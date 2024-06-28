import {NgModule} from '@angular/core';
import {CommonModule} from '@angular/common';
import {HomeComponent} from './page/home/home.component';
import {HomeRoutingModule} from "./home-routing.module";
import {BaseModule} from "../base/base.module";
import {GameCommonModule} from "../game-common/game-common.module";
import {VassalsComponent} from "./page/vassals/vassals.component";
import {BuildDialog} from "./dialogs/build/build.dialog";
import {UpgradeDialog} from "./dialogs/upgrade/upgrade.dialog";
import {FormsModule} from "@angular/forms";
import {VassalDetailsComponent} from './page/vassal-details/vassal-details.component';
import {LevelUpDialog} from "./dialogs/level-up/level-up.dialog";
import {MissionsComponent} from './page/missions/missions.component';
import {GoOnMissionDialog} from "./dialogs/go-on-mission/go-on-mission.dialog";
import {MissionResultsDialog} from './dialogs/mission-results/mission-results.dialog';
import {TreasuresComponent} from './page/treasures/treasures.component';
import {TreasureTitlePipe} from "./pipes/treasure-title.pipe";
import {MarkdownModule} from "ngx-markdown";
import {GoOnGenericHuntMissionDialog} from "./dialogs/go-on-generic-hunt-mission/go-on-generic-hunt-mission.dialog";
import {GoOnTimedMissionDialog} from "./dialogs/go-on-timed-mission/go-on-timed-mission.dialog";
import {StatusEffectCardComponent} from './components/status-effect-card/status-effect-card.component';
import {StatusEffectDialog} from "./dialogs/status-effect/status-effect.dialog";
import {RenameDialog} from "./dialogs/rename/rename.dialog";
import {WeaponNamePipe} from "./pipes/weapon-name.pipe";
import {WeaponDialog} from "./dialogs/weapon/weapon.dialog";
import {SpecializeDialog} from "./dialogs/specialize/specialize.dialog";
import {DescribePrimaryBonusPipe} from "./pipes/describe-primary-bonus.pipe";
import {DescribeSecondaryBonusPipe} from "./pipes/describe-secondary-bonus.pipe";
import {EquipWeaponDialog} from "./dialogs/equip-weapon/equip-weapon.dialog";
import {WeaponCardComponent} from './components/weapon-card/weapon-card.component';
import {SpendWillpowerDialog} from "./dialogs/spend-willpower/spend-willpower.dialog";
import {WillpowerOptionTitlePipe} from "./pipes/willpower-option-title.pipe";
import {WillpowerOptionDescriptionPipe} from "./pipes/willpower-option-description.pipe";
import {BuildingTitlePipe} from "./pipes/building-title.pipe";
import {RebuildDialog} from "./dialogs/rebuild/rebuild.dialog";
import {RepairWeaponDialog} from "./dialogs/repair-weapon/repair-weapon.dialog";
import {AddTagDialog} from "./dialogs/add-tag/add-tag.dialog";
import {FightGiantDialog} from "./dialogs/fight-giant/fight-giant.dialog";
import {ChanceOfSuccessComponent} from './components/chance-of-success/chance-of-success.component';
import {AttackDamageComponent} from "./components/attack-damage/attack-damage.component";
import {OpenTreasureChestDialog} from "./dialogs/open-treasure-chest/open-treasure-chest.dialog";
import {DecorateComponent} from './page/decorate/decorate.component';
import {DecorationBoxComponent} from './components/decoration-box/decoration-box.component';
import {RenameTownDialog} from "./dialogs/rename-town/rename-town.dialog";
import { MissionLogsComponent } from './page/mission-logs/mission-logs.component';
import {RetireVassalDialog} from "./dialogs/retire-vassal/retire-vassal.dialog";
import { FavoriteComponent } from './components/favorite/favorite.component';
import {DismissVassalDialog} from "./dialogs/dismiss-vassal/dismiss-vassal.dialog";
import {DecorationTitlePipe} from "./pipes/decoration-title.pipe";
import {UseBuildingPowerDialog} from "./dialogs/use-building-power/use-building-power.dialog";
import { ViewComponent } from './page/view/view.component';

@NgModule({
  declarations: [
    HomeComponent,
    VassalsComponent,
    BuildDialog,
    UpgradeDialog,
    VassalDetailsComponent,
    LevelUpDialog,
    MissionsComponent,
    GoOnMissionDialog,
    MissionResultsDialog,
    TreasuresComponent,
    TreasureTitlePipe,
    GoOnGenericHuntMissionDialog,
    GoOnTimedMissionDialog,
    StatusEffectCardComponent,
    StatusEffectDialog,
    RenameDialog,
    WeaponNamePipe,
    WeaponDialog,
    SpecializeDialog,
    DescribePrimaryBonusPipe,
    DescribeSecondaryBonusPipe,
    EquipWeaponDialog,
    WeaponCardComponent,
    SpendWillpowerDialog,
    WillpowerOptionTitlePipe,
    WillpowerOptionDescriptionPipe,
    BuildingTitlePipe,
    RebuildDialog,
    RepairWeaponDialog,
    AddTagDialog,
    FightGiantDialog,
    ChanceOfSuccessComponent,
    AttackDamageComponent,
    OpenTreasureChestDialog,
    DecorateComponent,
    DecorationBoxComponent,
    RenameTownDialog,
    MissionLogsComponent,
    RetireVassalDialog,
    FavoriteComponent,
    DismissVassalDialog,
    DecorationTitlePipe,
    UseBuildingPowerDialog,
    ViewComponent,
  ],
  exports: [],
  imports: [
    CommonModule,
    HomeRoutingModule,
    BaseModule,
    GameCommonModule,
    FormsModule,
    MarkdownModule,
  ]
})
export class HomeModule {
}
