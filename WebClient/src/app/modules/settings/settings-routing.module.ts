import {RouterModule, Routes} from "@angular/router";
import {NgModule} from "@angular/core";
import {SettingsComponent} from "./page/settings/settings.component";
import {SubscriptionsComponent} from "./page/subscriptions/subscriptions.component";
import {PassphraseComponent} from "./page/passphrase/passphrase.component";
import {EmailAddressComponent} from "./page/email-address/email-address.component";
import {AdvancedComponent} from "./page/advanced/advanced.component";

const routes: Routes = [
  { path: '', component: SettingsComponent },
  //{ path: 'subscriptions', component: SubscriptionsComponent },
  { path: 'passphrase', component: PassphraseComponent },
  { path: 'email', component: EmailAddressComponent },
  { path: 'advanced', component: AdvancedComponent }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule],
})
export class SettingsRoutingModule { }
