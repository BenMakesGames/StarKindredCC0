import { Pipe, PipeTransform } from '@angular/core';

@Pipe({
  name: 'treasureTitle'
})
export class TreasureTitlePipe implements PipeTransform {

  transform(value: string): string {
    switch(value)
    {
      case 'MagicHammer': return 'Magic Hammer';
      case 'Ichor': return 'Ichor';
      case 'RenamingScroll': return 'Naming Scroll';
      case 'BoxOfOres': return 'Box of Ores';
      case 'BasicChest': return 'Basic Chest';
      case 'BigBasicChest': return 'Big Basic Chest';
      case 'GoldChest': return 'Gold Chest';
      case 'RubyChest': return 'Ruby Chest';
      case 'TwilightChest': return 'Twilight Chest';
      case 'WeaponChest': return 'Weapon Chest';
      case 'TreasureMap': return 'Treasure Map';
      case 'WrappedSword': return 'Wrapped Sword';
      case 'Soma': return 'Soma';
      case 'Emerald': return 'Emerald';
      case 'CupOfLife': return 'Cup of Life';
      case 'CrystallizedQuint': return 'Crystallized Quint';
      case 'RallyingStandard': return 'Rallying Standard';
      case 'FishBag': return 'Fish Bag';
      default: return '???';
    }
  }

}
