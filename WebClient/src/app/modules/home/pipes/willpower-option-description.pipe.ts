import { Pipe, PipeTransform } from '@angular/core';

@Pipe({
  name: 'willpowerOptionDescription'
})
export class WillpowerOptionDescriptionPipe implements PipeTransform {

  transform(value: string): string {
    switch(value)
    {
      case 'Focus': return 'The next time this Vassal succeeds at a Mission, it will be a Great Success.';
      case 'LevelUp': return 'This Vassal will gain a level.';
      case 'GoldBuff': return 'The next time this Vassal completes a Mission and Gold is collected, +100% Gold is collected.';
      case 'QuintBuff': return 'The next time this Vassal creates a Decoration, or witnesses the creation of one, receive 1000 Quintesssence.';
      case 'LevelBuff': return 'This Vassal has +5 Levels for the purposes of hunting and fighting. **Duration:** 2 Missions';
      default: return '???';
    }
  }

}
