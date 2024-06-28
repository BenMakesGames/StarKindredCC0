import {Component, OnDestroy, OnInit} from '@angular/core';
import { ApiService } from "../../../base/services/api.service";
import { Router } from "@angular/router";
import {Subscription} from "rxjs";
import {TownResourcesService} from "../../../base/services/town-resources.service";
import {ResourceQuantityDto} from "../../../../dtos/resource-quantity.dto";
import { YieldOrientation } from 'src/app/modules/game-common/components/describe-yield/describe-yield.component';

@Component({
  selector: 'app-create-alliance',
  templateUrl: './create-alliance.component.html',
  styleUrls: ['./create-alliance.component.scss']
})
export class CreateAllianceComponent implements OnInit, OnDestroy {

  YieldOrientation = YieldOrientation;

  creating = false;
  resourcesSubscription = Subscription.EMPTY;
  resources: ResourceQuantityDto[]|null = null;

  constructor(private api: ApiService, private router: Router, private town: TownResourcesService) {
  }

  ngOnDestroy()
  {
    this.resourcesSubscription.unsubscribe();
  }

  ngOnInit(): void {
    this.resourcesSubscription = this.town.resources.subscribe({
      next: r => {
        if (r)
          this.resources = r;
        else
          this.town.reload();
      }
    });
  }

  doCreateAlliance()
  {
    if(this.creating)
      return;

    this.creating = true;

    this.api.post('alliances/create').subscribe({
      next: _ => {
        // noinspection JSIgnoredPromiseFromCall
        this.router.navigateByUrl('/alliance');

        this.town.update(this.resources!.map(r => r.type === 'Gold'
          ? { type: 'Gold', quantity: r.quantity - 1000 }
          : r
        ));
      },
      error: () => {
        this.creating = false;
      }
    });
  }

}
