import {Component, EventEmitter, Input, Output} from '@angular/core';
import {Router} from "@angular/router";

@Component({
  selector: 'app-vassal-filters',
  templateUrl: './vassal-filters.component.html',
  styleUrls: ['./vassal-filters.component.scss']
})
export class VassalFiltersComponent {

  @Input() tags!: { title: string, color: string }[];
  @Input() filters!: VassalSearchDto;
  @Output() filtersChange = new EventEmitter<VassalSearchDto>();

  @Input() href: string|null = null;

  constructor(private router: Router) { }

  doChange()
  {
    if(this.href)
    {
      // noinspection JSIgnoredPromiseFromCall
      this.router.navigate([this.href], {queryParams: this.filters});
    }
    else
      this.filtersChange.next(this.filters);
  }
}

export interface VassalSearchDto {
  name: string|null;
  element: string;
  nature: string;
  sign: string;
  tag: string;
  onMission: boolean|null;
  isLeader: boolean|null;
  page: number;
  pageSize: number;
}
