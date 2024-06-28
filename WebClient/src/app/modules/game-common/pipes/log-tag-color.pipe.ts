import { Pipe, PipeTransform } from '@angular/core';

@Pipe({
  name: 'logTagColor'
})
export class LogTagColorPipe implements PipeTransform {

  transform(value: string): string {
    switch(value)
    {
      case 'Oracle': return 'purple';
      case 'CompleteMission': return '#369';
      case 'Success': return '#0c0';
      case 'GreatSuccess': return '#0cf';
      case 'AccountActivity': return '#a03';
      case 'UpdatedAvatarColor': return '#fa0';
      case 'TreasureHunt': return '#fa0';
      case 'MonsterHunt': return '#f00';
      case 'AnimalHunt': return '#960';
      case 'Story': return '#806';
      case 'Gather': return '#080';
      case 'LoggedIn': return '#ccc';
      case 'SpentWillpower': return '#fa0';
      case 'LeveledUpVassal': return '#a0a';
      default: return '#333';
    }
  }
}
