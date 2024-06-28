import {CUSTOM_ELEMENTS_SCHEMA, NgModule} from '@angular/core';
import { CommonModule } from '@angular/common';
import { SettingsComponent } from './page/settings/settings.component';
import {GameCommonModule} from "../game-common/game-common.module";
import {FormsModule} from "@angular/forms";
import {SettingsRoutingModule} from "./settings-routing.module";
import {BaseModule} from "../base/base.module";
import { AdvancedComponent } from './page/advanced/advanced.component';
import { PassphraseComponent } from './page/passphrase/passphrase.component';
import { EmailAddressComponent } from './page/email-address/email-address.component';
import { SubscriptionsComponent } from './page/subscriptions/subscriptions.component';

@NgModule({
  declarations: [
    SettingsComponent,
    AdvancedComponent,
    PassphraseComponent,
    EmailAddressComponent,
    SubscriptionsComponent,
  ],
  imports: [
    CommonModule,
    GameCommonModule,
    FormsModule,
    SettingsRoutingModule,
    BaseModule
  ],
  schemas: [ CUSTOM_ELEMENTS_SCHEMA ]
})
export class SettingsModule { }
