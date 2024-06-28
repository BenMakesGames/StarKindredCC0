import {RouterModule, Routes} from "@angular/router";
import {NgModule} from "@angular/core";
import { HomeComponent } from "./page/home/home.component";
import { VassalsComponent } from "./page/vassals/vassals.component";
import { VassalDetailsComponent } from "./page/vassal-details/vassal-details.component";
import { MissionsComponent } from "./page/missions/missions.component";
import {TreasuresComponent} from "./page/treasures/treasures.component";
import {DecorateComponent} from "./page/decorate/decorate.component";
import {MissionLogsComponent} from "./page/mission-logs/mission-logs.component";
import {ViewComponent} from "./page/view/view.component";

const routes: Routes = [
  { path: '', component: HomeComponent },
  { path: 'logs', component: MissionLogsComponent },
  { path: 'decorate', component: DecorateComponent },
  { path: 'vassals', component: VassalsComponent },
  { path: 'vassals/:id', component: VassalDetailsComponent },
  { path: 'missions', component: MissionsComponent },
  { path: 'treasures', component: TreasuresComponent },
  { path: 'view/:id', component: ViewComponent },
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule],
})
export class HomeRoutingModule { }
