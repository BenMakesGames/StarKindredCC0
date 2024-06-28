import { Injectable } from '@angular/core';
import { BehaviorSubject } from "rxjs";
import { ResourceQuantityDto } from "../../../dtos/resource-quantity.dto";
import { ApiService } from "./api.service";

@Injectable({
  providedIn: 'root'
})
export class TownResourcesService {

  resources = new BehaviorSubject<ResourceQuantityDto[]|null>(null);

  constructor(private api: ApiService) { }

  reload()
  {
    this.api.get<{ resources: ResourceQuantityDto[] }>('towns/my').subscribe({
      next: r => {
        this.resources.next(r.data.resources);
      }
    });
  }

  update(resources: ResourceQuantityDto[])
  {
    this.resources.next(resources);
  }

  earn(earnings: ResourceQuantityDto[])
  {
    if(!this.resources.value)
      return;

    this.resources.next(this.resources.value.map(r => {
      const gain = earnings.find(e => e.type == r.type);

      if(!gain) return r;

      return {
        quantity: r.quantity + gain.quantity,
        type: r.type
      };
    }));
  }

  spend(cost: ResourceQuantityDto[])
  {
    if(!this.resources.value)
      return;

    this.resources.next(this.resources.value.map(r => {
      const c = cost.find(c => c.type == r.type);

      if(!c) return r;

      return {
        quantity: r.quantity - c.quantity,
        type: r.type
      };
    }));
  }
}
