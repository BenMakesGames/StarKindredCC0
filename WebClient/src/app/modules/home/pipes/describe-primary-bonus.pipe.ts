import { Pipe, PipeTransform } from '@angular/core';

@Pipe({
  name: 'describePrimaryBonus'
})
export class DescribePrimaryBonusPipe implements PipeTransform {

  transform(bonus: string, level: number): string {
    const bonusLevel = Math.floor(level / 2);
    return DescribePrimaryBonusPipe.describe(bonus, bonusLevel);
  }

  static describe(bonus: string, bonusLevel: number): string
  {
    switch(bonus)
    {
      case 'HuntingLevels': return 'When hunting animals, monsters, and Giants, Vassal hunts as if ' + [2, 5, 10][bonusLevel] + ' levels higher.';
      case 'FasterMissions': return 'Complete missions ' + [5, 7, 10][bonusLevel] + '% faster.';
      case 'MoreGold': return 'When a mission yields Gold, get ' + [5, 10, 20][bonusLevel] + '% more Gold.';
      case 'MeatGetsWood': return 'When an animal or monster yields Meat, get +' + [10, 25, 50][bonusLevel] + '% as much Wood.';
      case 'GoldGetsWine': return 'When a mission yields Gold, get +' + [10, 25, 50][bonusLevel] + '% as much Wine.';
      case 'WeaponsGetWheat': return 'When a mission yields a Weapon, get Wheat equal to ' + [100, 120, 150][bonusLevel] + '% the mission\'s Level.';
      case 'RecruitBonus': return 'When recruiting, new recruits start with +' + [2, 5, 10][bonusLevel] + ' levels.';
      default: return '???';
    }
  }

}
