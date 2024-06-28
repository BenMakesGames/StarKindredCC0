import {RouterModule, Routes} from "@angular/router";
import {NgModule} from "@angular/core";
import { AllianceComponent } from "./page/alliance/alliance.component";
import { CreateAllianceComponent } from "./page/create-alliance/create-alliance.component";
import {AllianceLogsComponent} from "./page/alliance-logs/alliance-logs.component";
import {IndexComponent} from "./page/index/index.component";
import {ViewComponent} from "./page/view/view.component";
import {RanksComponent} from "./page/ranks/ranks.component";

const routes: Routes = [
  { path: '', component: AllianceComponent },
  { path: 'titles', component: RanksComponent },
  { path: 'search', component: IndexComponent },
  { path: 'logs', component: AllianceLogsComponent },
  { path: 'create', component: CreateAllianceComponent },
  { path: 'view/:id', component: ViewComponent },
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule],
})
export class AllianceRoutingModule { }
