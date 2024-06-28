import { Pipe, PipeTransform } from '@angular/core';

@Pipe({
  name: 'logTagTitle'
})
export class LogTagTitlePipe implements PipeTransform {

  transform(value: string): string {
    switch(value)
    {
      case 'AccountActivity': return 'Account Activity';
      case 'UpdatedAvatarColor': return 'Updated Avatar Color';
      case 'CompleteMission': return 'Completed Mission';
      case 'GreatSuccess': return 'Great Success';
      case 'LoggedIn': return 'Logged In';
      case 'BoatTour': return 'Boat Tour';
      case 'TreasureHunt': return 'Treasure Hunt';
      case 'MonsterHunt': return 'Monster Hunt';
      case 'AnimalHunt': return 'Animal Hunt';
      case 'UpdatedEmail': return 'Updated Email';
      case 'UpdatedPassphrase': return 'Updated Passphrase';
      case 'UpdatedAvatarImage': return 'Updated Profile Picture';
      case 'SpentWillpower': return 'Spent Willpower';
      case 'LeveledUpVassal': return 'Leveled Up Vassal';
      default: return value;
    }
  }
}
