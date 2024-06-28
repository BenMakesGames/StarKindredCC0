import {Component, Input} from '@angular/core';
import {HSL} from "../../../../dtos/hsl.model";

@Component({
  selector: 'app-member-card',
  templateUrl: './member-card.component.html',
  styleUrls: ['./member-card.component.scss']
})
export class MemberCardComponent {
  @Input() name!: string;
  @Input() avatar!: string;
  @Input() color!: HSL;
  @Input() level: number|undefined;
  @Input() isLeader!: boolean;
  @Input() rank: string|null|undefined;
}
