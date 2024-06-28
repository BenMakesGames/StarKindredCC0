import {Component, Input, OnChanges, SimpleChanges} from '@angular/core';
import {VassalSearchResultModel} from "../../../../dtos/vassal-search-result.model";
import {computeChanceOfSuccess, computeChanceOfSuccessWithElement} from "../../helpers/mission-math";

@Component({
  selector: 'app-chance-of-success',
  templateUrl: './chance-of-success.component.html',
  styleUrls: ['./chance-of-success.component.scss']
})
export class ChanceOfSuccessComponent implements OnChanges {

  @Input() vassals!: VassalSearchResultModel[];
  @Input() maxVassals!: number;
  @Input() level!: number;
  @Input() element: string|null = null;
  @Input() applyHuntingBonus: boolean = false;

  chanceOfSuccess = 0;

  ngOnChanges(changes: SimpleChanges) {
    if(this.element)
      this.chanceOfSuccess = computeChanceOfSuccessWithElement(this.vassals, this.element, this.level, this.maxVassals);
    else
      this.chanceOfSuccess = computeChanceOfSuccess(this.vassals, this.level, this.maxVassals, this.applyHuntingBonus);
  }
}
