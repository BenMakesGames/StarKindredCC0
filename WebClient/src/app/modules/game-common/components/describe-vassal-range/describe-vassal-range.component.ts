import {Component, Input} from '@angular/core';

@Component({
  selector: 'app-describe-vassal-range',
  templateUrl: './describe-vassal-range.component.html',
  styleUrls: ['./describe-vassal-range.component.scss']
})
export class DescribeVassalRangeComponent {

  @Input() min!: number;
  @Input() max!: number;
  @Input() requiredElement: string|null = null;

}
