import {Component, Input, OnChanges, SimpleChanges} from '@angular/core';
import {ResourceQuantityDto} from "../../../../dtos/resource-quantity.dto";
import {RollingResourceQuantity} from "../../models/rolling-resource-quantity";

@Component({
  selector: 'app-resources',
  templateUrl: './resources.component.html',
  styleUrls: ['./resources.component.scss']
})
export class ResourcesComponent implements OnChanges {

  @Input() resources!: ResourceQuantityDto[];

  rollingResources: RollingResourceQuantity[] = [];

  ngOnChanges(changes: SimpleChanges) {
    let resources = changes['resources'];

    if(resources.firstChange)
    {
      this.rollingResources = resources.currentValue.map((resource: ResourceQuantityDto) => {
        return {
          type: resource.type,
          oldQuantity: resource.quantity,
          newQuantity: resource.quantity,
        }
      });
    }
    else
    {
      this.rollingResources = resources.currentValue.map((resource: ResourceQuantityDto) => {
        let oldValue = resources.previousValue.find((oldResource: ResourceQuantityDto) => oldResource.type === resource.type)?.quantity ?? 0;

        return {
          type: resource.type,
          oldQuantity: oldValue,
          newQuantity: resource.quantity,
        }
      });
    }
  }
}
