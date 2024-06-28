import {Component, EventEmitter, Input, OnDestroy, OnInit, Output} from '@angular/core';
import {Subscription} from "rxjs";
import {PaginatedResultsDto} from "../../../../dtos/paginated-results.dto";
import {VassalSearchResultModel} from "../../../../dtos/vassal-search-result.model";
import {ApiService} from "../../../base/services/api.service";
import {VassalSearchDto} from "../vassal-filters/vassal-filters.component";

@Component({
  selector: 'app-select-vassals',
  templateUrl: './select-vassals.component.html',
  styleUrls: ['./select-vassals.component.scss']
})
export class SelectVassalsComponent implements OnInit, OnDestroy {

  @Input() disabled = false;
  @Input() canAdd: (v: VassalSearchResultModel) => boolean = _ => true;
  @Input() tags!: { title: string, color: string }[];
  @Input() extraInfo: ((v: VassalSearchResultModel) => string)|null = null;
  @Output() selectVassal = new EventEmitter<VassalSearchResultModel>();

  searchAjax = Subscription.EMPTY;
  public vassals: PaginatedResultsDto<VassalSearchResultModel>|null = null;

  query: VassalSearchDto = {
    name: null,
    element: '',
    nature: '',
    sign: '',
    tag: '',
    onMission: false,
    isLeader: false,
    page: 1,
    pageSize: 30
  };

  constructor(private api: ApiService) { }

  ngOnInit(): void {
    this.doSearch();
  }

  ngOnDestroy()
  {
    this.searchAjax.unsubscribe();
  }

  doUpdateFilters(filters: VassalSearchDto)
  {
    this.query = { ...filters, page: 1 };
    this.doSearch();
  }

  doChangePage(newPage: number)
  {
    this.query.page = newPage;
    this.doSearch();
  }

  doSearch()
  {
    this.searchAjax.unsubscribe();

    this.searchAjax = this.api.get<PaginatedResultsDto<VassalSearchResultModel>>('vassals/search', this.query).subscribe({
      next: r => {
        this.vassals = r.data;
      }
    });
  }

  doAddVassal(vassal: VassalSearchResultModel)
  {
    this.selectVassal.next(vassal);
  }

}
