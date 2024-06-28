import {Component, OnInit} from '@angular/core';
import {ApiService} from "../../../base/services/api.service";
import {ActivatedRoute} from "@angular/router";
import {PaginatedResultsDto} from "../../../../dtos/paginated-results.dto";

@Component({
  selector: 'app-subscriptions',
  templateUrl: './subscriptions.component.html',
  styleUrls: ['./subscriptions.component.scss']
})
export class SubscriptionsComponent implements OnInit {

  subscriptions: PaginatedResultsDto<SubscriptionDto>|null = null;
  query: any = { page: 1 };

  userId: string|null = null;

  constructor(private api: ApiService, private activatedRoute: ActivatedRoute) { }

  ngOnInit(): void {
    this.api.get<{ id: string }>('accounts/info').subscribe({
      next: r => {
        this.userId = r.data.id;
      }
    });

    this.activatedRoute.queryParamMap.subscribe({
      next: params => {
        this.query = {
          ...this.query,
          page: parseInt(params.get('page') ?? '1')
        };

        this.loadSubscriptions();
      }
    });
  }

  private loadSubscriptions()
  {
    this.api.get<PaginatedResultsDto<SubscriptionDto>>('accounts/subscriptions', this.query).subscribe({
      next: r => {
        this.subscriptions = r.data;
      }
    });
  }

}

interface SubscriptionDto
{
  subscriptionService: string;
  startDate: string;
  endDate: string;
}
