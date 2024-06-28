import { Component, OnInit } from '@angular/core';
import {ApiService} from "../../../base/services/api.service";
import {PaginatedResultsDto} from "../../../../dtos/paginated-results.dto";

@Component({
  templateUrl: './list.component.html',
  styleUrls: ['./list.component.scss']
})
export class ListComponent implements OnInit {

  query = { page: 1 };
  stories: PaginatedResultsDto<Story>|null = null;

  constructor(private api: ApiService) { }

  ngOnInit(): void {
    this.loadStories();
  }

  public doChangePage(page: number)
  {
    this.query = { ...this.query, page: page };
    this.loadStories();
  }

  private loadStories()
  {
    this.api.get<PaginatedResultsDto<Story>>('stories', this.query).subscribe({
      next: r => {
        this.stories = r.data;
      }
    });
  }

}

interface Story
{
  id: string;
  year: number;
  month: number;
  title: string;
  summary: string;
  missionsAvailable: number;
  missionsComplete: number;
}
