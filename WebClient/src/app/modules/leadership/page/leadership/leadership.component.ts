import {Component, OnDestroy, OnInit} from '@angular/core';
import {ApiService} from "../../../base/services/api.service";
import {TownDto} from "../../../../dtos/town.dto";
import {Subscription} from "rxjs";

@Component({
  selector: 'app-leadership',
  templateUrl: './leadership.component.html',
  styleUrls: ['./leadership.component.scss']
})
export class LeadershipComponent implements OnInit, OnDestroy {

  townSubscription = Subscription.EMPTY;
  town: TownDto|null = null;

  constructor(private api: ApiService) { }

  ngOnInit(): void {
    this.townSubscription = this.api.get<TownDto>('towns/my').subscribe({
      next: t => this.town = t.data
    });
  }

  ngOnDestroy() {
    this.townSubscription.unsubscribe();
  }

}
