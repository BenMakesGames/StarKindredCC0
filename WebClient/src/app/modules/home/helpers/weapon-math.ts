export interface WeaponInfo
{
  level: number;
  primaryBonus: string;
  secondaryBonus: string|null;
}

export function weaponBonusHuntingLevels(weapon: WeaponInfo|null): number
{
  const effectiveLevel = weaponBonusLevel(weapon, 'HuntingLevels');

  switch(effectiveLevel)
  {
    case 0: return 0;
    case 1: return 2;
    case 2: return 5;
    case 3: return 10;
    default: throw 'Level must be between 0 and 3.';
  }
}

export function weaponBonusLevel(weapon: WeaponInfo|null, bonus: string): number
{
  if(!weapon)
    return 0;

  if(weapon.primaryBonus == bonus)
    return weaponPrimaryBonusLevel(weapon.level);

  if (weapon.secondaryBonus == bonus)
    return weaponSecondaryBonusLevel(weapon.level);

  return 0;
}

export function weaponPrimaryBonusLevel(level: number): number
{
  return Math.floor(level / 2) + 1;
}

export function weaponSecondaryBonusLevel(level: number): number
{
  switch(level)
  {
    case 1: case 2: return 0;
    case 3: case 4: return 1;
    case 5: return 2;
    default: throw 'Level must be between 1 and 5.';
  }
}
