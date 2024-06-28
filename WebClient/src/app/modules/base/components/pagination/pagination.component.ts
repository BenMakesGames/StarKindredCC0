import {Component, EventEmitter, Input, OnChanges, Output, SimpleChanges} from '@angular/core';

@Component({
  selector: 'app-pagination',
  templateUrl: './pagination.component.html',
  styleUrls: ['./pagination.component.scss']
})
export class PaginationComponent implements OnChanges {

  @Output() pageChange = new EventEmitter<number>();

  @Input() page!: number;
  @Input() totalPages!: number;
  @Input() href: string|null = null;
  @Input() queryParams: any = {};

  nextQuery: any = {};
  prevQuery: any = {};

  ngOnChanges(changes: SimpleChanges) {
    if('page' in changes || 'queryParams' in changes) {
      this.nextQuery = { ...this.queryParams, page: this.page + 1 };
      this.prevQuery = { ...this.queryParams, page: this.page - 1 };
    }
  }
}
