import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import {HomeComponent} from "./page/home/home.component";
import {CreateAccountComponent} from "./page/create-account/create-account.component";
import {NotFoundComponent} from "./page/not-found/not-found.component";
import {MagicLoginComponent} from "./page/magic-login/magic-login.component";

const routes: Routes = [
  { path: '', component: HomeComponent },
  { path: 'createAccount', component: CreateAccountComponent },
  { path: 'magicLogin', component: MagicLoginComponent },

  { path: 'home', loadChildren: () => import('./modules/home/home.module').then(m => m.HomeModule), canActivate: [ ] },
  { path: 'leadership', loadChildren: () => import('./modules/leadership/leadership.module').then(m => m.LeadershipModule), canActivate: [ ] },
  { path: 'stories', loadChildren: () => import('./modules/stories/stories.module').then(m => m.StoriesModule), canActivate: [ ] },
  { path: 'settings', loadChildren: () => import('./modules/settings/settings.module').then(m => m.SettingsModule), canActivate: [ ] },
  { path: 'alliance', loadChildren: () => import('./modules/alliance/alliance.module').then(m => m.AllianceModule), canActivate: [ ] },
  { path: 'news', loadChildren: () => import('./modules/news/news.module').then(m => m.NewsModule), canActivate: [ ] },
  { path: '**', component: NotFoundComponent }
];

@NgModule({
  imports: [RouterModule.forRoot(routes, { scrollPositionRestoration: 'enabled' })],
  exports: [RouterModule]
})
export class AppRoutingModule { }
