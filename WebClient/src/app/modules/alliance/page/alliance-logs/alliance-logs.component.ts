import { Component, OnInit } from '@angular/core';
import {PaginatedResultsDto} from "../../../../dtos/paginated-results.dto";
import {ApiService} from "../../../base/services/api.service";
import {ActivatedRoute} from "@angular/router";

@Component({
  templateUrl: './alliance-logs.component.html',
  styleUrls: ['./alliance-logs.component.scss']
})
export class AllianceLogsComponent implements OnInit {

  query = {
    page: 1
  };

  results: PaginatedResultsDto<LogDto>|null = null;

  constructor(private api: ApiService, private activatedRoute: ActivatedRoute) {
  }

  ngOnInit(): void {
    this.activatedRoute.queryParamMap.subscribe({
      next: params => {
        this.query = {
          ...this.query,
          page: parseInt(params.get('page') ?? '1')
        };

        this.loadLogs();
      }
    });
  }

  loadLogs()
  {
    this.api.get<PaginatedResultsDto<LogDto>>('alliances/my/logs', this.query).subscribe({
      next: r => {
        this.results = r.data;
      }
    });
  }
}

interface LogDto
{
  createdOn: string;
  activityType: string;
  message: string;
}
