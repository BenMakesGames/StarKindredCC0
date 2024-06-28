import {Component, OnDestroy, OnInit} from '@angular/core';
import {DecorationDto, TownDto} from "../../../../dtos/town.dto";
import {ActivatedRoute} from "@angular/router";
import {Subscription} from "rxjs";
import {ApiService} from "../../../base/services/api.service";
import {BuildingDto} from "../../../../dtos/building.dto";
import {TownPositions} from "../../helpers/town-positions";

@Component({
  selector: 'app-view',
  templateUrl: './view.component.html',
  styleUrls: ['./view.component.scss']
})
export class ViewComponent implements OnInit, OnDestroy {

  town: ResponseDto|null = null;

  paramSubscription = Subscription.EMPTY;
  townId: string| null = null;

  TownPositions = TownPositions;

  constructor(private activatedRoute: ActivatedRoute, private api: ApiService) { }

  ngOnInit(): void {
    this.paramSubscription = this.activatedRoute.paramMap.subscribe(params => {
      this.townId = params.get('id')!;

      this.loadTown();
    });
  }

  ngOnDestroy() {
    this.paramSubscription.unsubscribe();
  }

  private loadTown() {
    this.api.get<ResponseDto>(`towns/view/${this.townId}`).subscribe({
      next: response => {
        this.town = response.data;
      }
    });
  }
}

interface ResponseDto {
  buildings: { position: number, type: string, level: number }[];
  decorations: DecorationDto[];
}
