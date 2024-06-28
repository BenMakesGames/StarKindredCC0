import { Pipe, PipeTransform } from '@angular/core';

@Pipe({
  name: 'willpowerOptionTitle'
})
export class WillpowerOptionTitlePipe implements PipeTransform {

  transform(value: string): string {
    switch(value)
    {
      case 'Focus': return 'Focus';
      case 'LevelUp': return 'Level Up';
      case 'GoldBuff': return 'Golden Touch';
      case 'QuintBuff': return 'Artistic Vision';
      case 'LevelBuff': return 'Strength';
      default: return '???';
    }
  }

}
