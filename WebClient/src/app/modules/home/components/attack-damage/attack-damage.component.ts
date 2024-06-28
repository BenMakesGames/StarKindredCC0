import {Component, Input, OnChanges, SimpleChanges} from '@angular/core';
import {VassalSearchResultModel} from "../../../../dtos/vassal-search-result.model";
import {computeAttackDamage} from "../../helpers/mission-math";

@Component({
  selector: 'app-attack-damage',
  templateUrl: './attack-damage.component.html',
  styleUrls: ['./attack-damage.component.scss']
})
export class AttackDamageComponent implements OnChanges {

  @Input() vassals!: VassalSearchResultModel[];
  @Input() element!: string;

  damage = 0;

  ngOnChanges(changes: SimpleChanges) {
    this.damage = computeAttackDamage(this.vassals, this.element);
  }
}
