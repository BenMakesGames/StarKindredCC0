import { Component, OnInit } from '@angular/core';
import {ApiService} from "../../../base/services/api.service";
import {PaginatedResultsDto} from "../../../../dtos/paginated-results.dto";
import {ActivatedRoute} from "@angular/router";

@Component({
  selector: 'app-mission-logs',
  templateUrl: './mission-logs.component.html',
  styleUrls: ['./mission-logs.component.scss']
})
export class MissionLogsComponent implements OnInit {

  query = { page: 1 };
  logs: PaginatedResultsDto<LogDto>|null = null;

  constructor(private api: ApiService, private activatedRoute: ActivatedRoute) { }

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

  private loadLogs()
  {
    this.api.get<PaginatedResultsDto<LogDto>>('accounts/logs', this.query).subscribe({
      next: r => {
        this.logs = r.data;
      }
    })
  }
}

interface LogDto
{
  message: string;
  tags: string[];
  date: string;
}
