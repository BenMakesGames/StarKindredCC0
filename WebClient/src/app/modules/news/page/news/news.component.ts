import { Component, OnInit } from '@angular/core';
import { ApiService } from "../../../base/services/api.service";
import { PaginatedResultsDto } from "../../../../dtos/paginated-results.dto";
import {ActivatedRoute} from "@angular/router";
import {NewNewsService} from "../../../base/services/new-news.service";

@Component({
  selector: 'app-news',
  templateUrl: './news.component.html',
  styleUrls: ['./news.component.scss']
})
export class NewsComponent implements OnInit {

  news: PaginatedResultsDto<AnnouncementDto>|null = null;

  query = { page: 1 };

  constructor(private api: ApiService, private activatedRoute: ActivatedRoute, private newNews: NewNewsService) { }

  ngOnInit(): void {
    this.activatedRoute.queryParamMap.subscribe({
      next: params => {
        this.query = {
          ...this.query,
          page: parseInt(params.get('page') ?? '1')
        };

        this.loadAnnouncements();
      }
    });
  }

  private loadAnnouncements()
  {
    this.api.get<PaginatedResultsDto<AnnouncementDto>>('announcements', this.query).subscribe({
      next: r => {
        this.news = r.data;
        this.newNews.unreadNews.next(false);
      }
    });
  }

}

interface AnnouncementDto
{
  createdOn: string;
  type: string;
  body: string;
}
