import { Component, OnDestroy, OnInit } from '@angular/core';
import { ApiService } from "../../../base/services/api.service";
import { PaginatedResultsDto } from "../../../../dtos/paginated-results.dto";
import {VassalSearchResultModel} from "../../../../dtos/vassal-search-result.model";
import { Subscription } from "rxjs";
import { TownResourcesService } from "../../../base/services/town-resources.service";
import { ResourceQuantityDto } from "../../../../dtos/resource-quantity.dto";
import {ActivatedRoute, Router} from "@angular/router";
import {VassalSearchDto} from "../../../game-common/components/vassal-filters/vassal-filters.component";

@Component({
  templateUrl: './vassals.component.html',
  styleUrls: ['./vassals.component.scss']
})
export class VassalsComponent implements OnInit, OnDestroy {

  resourcesSubscription = Subscription.EMPTY;

  query: VassalSearchDto = { name: '', element: '', nature: '', sign: '', tag: '', onMission: null, isLeader: null, page: 1, pageSize: 60 };
  resources: ResourceQuantityDto[]|null = null;

  vassals: PaginatedResultsDto<VassalSearchResultModel>|null = null;
  tags: TagDto[]|null = null;

  constructor(
    private api: ApiService, private town: TownResourcesService, private router: Router,
    private activatedRoute: ActivatedRoute
  )
  {

  }

  ngOnDestroy()
  {
    this.resourcesSubscription.unsubscribe();
  }

  ngOnInit(): void {
    this.resourcesSubscription = this.town.resources.subscribe({
      next: r => {
        if(r)
          this.resources = r;
        else
          this.town.reload();
      }
    });

    this.api.get<{ tags: TagDto[] }>('accounts/tags').subscribe({
      next: r => {
        this.tags = r.data.tags;
      }
    });

    this.activatedRoute.queryParamMap.subscribe({
      next: params => {
        this.query = {
          ...this.query,
          tag: params.get('tag') ?? '',
          element: params.get('element') ?? '',
          nature: params.get('nature') ?? '',
          sign: params.get('sign') ?? '',
          name: params.get('name') ?? '',
          onMission: this.parseBool(params.get('onMission')),
          page: parseInt(params.get('page') ?? '1')
        };

        this.loadVassals();
      }
    });
  }

  parseBool(value: string|null|undefined): boolean|null
  {
    if(value === 'true' || value === '1') return true;
    if(value === 'false' || value === '0') return false;
    return null;
  }

  private loadVassals()
  {
    this.api.get<PaginatedResultsDto<VassalSearchResultModel>>('vassals/search', this.query).subscribe({
      next: r => {
        this.vassals = r.data;
      }
    });
  }
}

interface TagDto
{
  title: string;
  color: string;
  vassalCount: number;
}
