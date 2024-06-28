import {Component, Input} from '@angular/core';
import {ResourceQuantityDto} from "../../../../dtos/resource-quantity.dto";

@Component({
  selector: 'app-resource',
  templateUrl: './resource.component.html',
  styleUrls: ['./resource.component.scss']
})
export class ResourceComponent {

  @Input() resource!: ResourceQuantityDto;

}
