import { Component, OnInit } from '@angular/core';
import {ApiService} from "../../../base/services/api.service";
import {PaginatedResultsDto} from "../../../../dtos/paginated-results.dto";
import {ActivatedRoute, Router} from "@angular/router";
import {HSL} from "../../../../dtos/hsl.model";

@Component({
  templateUrl: './index.component.html',
  styleUrls: ['./index.component.scss']
})
export class IndexComponent implements OnInit {

  query = { page: 1 };

  results: PaginatedResultsDto<AllianceDto>|null = null;

  constructor(private api: ApiService, private activatedRoute: ActivatedRoute, private router: Router) { }

  ngOnInit(): void {
    this.activatedRoute.queryParamMap.subscribe({
      next: params => {
        this.query = {
          ...this.query,
          page: parseInt(params.get('page') ?? '1')
        };

        this.loadAlliances();
      }
    });
  }

  doViewAlliance(alliance: AllianceDto)
  {
    // noinspection JSIgnoredPromiseFromCall
    this.router.navigateByUrl('/alliance/view/' + alliance.id);
  }

  private loadAlliances()
  {
    this.api.get<PaginatedResultsDto<AllianceDto>>('alliances', this.query).subscribe({
      next: r => {
        this.results = r.data;
      }
    });
  }
}

interface AllianceDto
{
  id: string;
  leader: { name: string, avatar: string, color: HSL };
  createdOn: string;
  lastActiveOn: string;
  level: number;
  memberCount: number;
  openInvitation: { minLevel: number, maxLevel: number }|null;
}
