import {Component} from '@angular/core';
import {SwUpdate} from "@angular/service-worker";

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.scss']
})
export class AppComponent {
  broken = false;

  constructor(private updates: SwUpdate) {
    updates.unrecoverable.subscribe({
      next: _ => {
        this.broken = true;
      }
    });

    updates.versionUpdates.subscribe({
      next: e => {
        if(e.type == 'VERSION_READY')
          updates.activateUpdate().then(() => document.location.reload());
      }
    });
  }
}
