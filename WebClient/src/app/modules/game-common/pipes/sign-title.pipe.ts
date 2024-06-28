import { Pipe, PipeTransform } from '@angular/core';

@Pipe({
  name: 'signTitle'
})
export class SignTitlePipe implements PipeTransform {

  transform(value: string): string {
    switch(value)
    {
      case 'EightPlants': return 'Eight Plants';
      case 'DoubleTrident': return 'Double-trident';
      case 'LargeCupAndLittleCup': return 'Large Cup & Little Cup';
      case 'PapyrusBoat': return 'Papyrus Boat';
      case 'PanFlute': return 'Pan Flute';
      default: return value;
    }
  }
}
