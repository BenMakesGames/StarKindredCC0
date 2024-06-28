import {RouterModule, Routes} from "@angular/router";
import {NgModule} from "@angular/core";
import {LeadershipComponent} from "./page/leadership/leadership.component";

const routes: Routes = [
  { path: '', component: LeadershipComponent },
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule],
})
export class LeadershipRoutingModule { }
