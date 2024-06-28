import {Component, Input, OnChanges, SimpleChanges} from '@angular/core';
import {RollingResourceQuantity} from "../../models/rolling-resource-quantity";

@Component({
  selector: 'app-rolling-resource',
  templateUrl: './rolling-resource.component.html',
  styleUrls: ['./rolling-resource.component.scss']
})
export class RollingResourceComponent implements OnChanges {

  @Input() resource!: RollingResourceQuantity;

  delta = 0;

  ngOnChanges(changes: SimpleChanges): void {
    const resource = changes['resource'];

    if(resource) {
      this.delta = (<RollingResourceQuantity>resource.currentValue).newQuantity - (<RollingResourceQuantity>resource.currentValue).oldQuantity;
    }
  }
}
