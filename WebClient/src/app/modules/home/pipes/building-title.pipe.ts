import { Pipe, PipeTransform } from '@angular/core';

@Pipe({
  name: 'buildingTitle'
})
export class BuildingTitlePipe implements PipeTransform {

  transform(value: string): string {
    switch(value)
    {
      case 'StonePit': return 'Stone Pit';
      case 'GoldMine': return 'Gold Mine';
      case 'IronMine': return 'Iron Mine';
      case 'MarbleQuarry': return 'Marble Quarry';
      case 'TradeDepot': return 'Trade Depot';
      default: return value;
    }
  }
}
