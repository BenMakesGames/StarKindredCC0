import { Pipe, PipeTransform } from '@angular/core';

@Pipe({
  name: 'weaponName'
})
export class WeaponNamePipe implements PipeTransform {

  transform(value: string): string {
    switch(value)
    {
      case 'HuntingLevels': return 'Sword';
      case 'FasterMissions': return 'Horn';
      case 'MoreGold': return 'Shovel';
      case 'MeatGetsWood': return 'Axe';
      case 'GoldGetsWine': return 'Wand';
      case 'WeaponsGetWheat': return 'Scythe';
      case 'RecruitBonus': return 'Lyre';
      default: return '???';
    }
  }

}
