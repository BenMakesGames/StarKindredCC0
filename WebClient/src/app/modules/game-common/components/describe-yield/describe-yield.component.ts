import { Component, Input } from '@angular/core';
import {ResourceQuantityDto} from "../../../../dtos/resource-quantity.dto";

@Component({
  selector: 'app-describe-yield',
  templateUrl: './describe-yield.component.html',
  styleUrls: ['./describe-yield.component.scss']
})
export class DescribeYieldComponent
{
  YieldOrientation = YieldOrientation;

  @Input() yield!: ResourceQuantityDto[];
  @Input() orientation: YieldOrientation = YieldOrientation.Stacked;
}

export enum YieldOrientation
{
  Vertical = 'vertical',
  Horizontal = 'horizontal',
  Stacked = 'stacked'
}
