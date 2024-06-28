import {weaponBonusHuntingLevels, WeaponInfo} from "./weapon-math";

function fractionAsPercent(fraction: number): number
{
  if(fraction >= 1.5)
    return 100;
  else if(fraction >= 1)
    return 80;
  else if(fraction >= 0.5)
    return 50;
  else
    return 10;
}

export function computeChanceOfSuccess(vassals: { level: number, weapon: WeaponInfo|null }[], level: number, maxVassals: number, applyHuntingBonus: boolean): number
{
  if(vassals.length == 0)
    return 0;

  const totalVassalLevel = applyHuntingBonus
    ? vassals.reduce((p, v) => p + weaponBonusHuntingLevels(v.weapon) + v.level + 1, 0)
    : vassals.reduce((p, v) => p + v.level + 1, 0)
  ;

  const fraction = totalVassalLevel / (level + maxVassals);

  return fractionAsPercent(fraction);
}

export function computeChanceOfSuccessWithElement(vassals: { level: number, element: string, weapon: WeaponInfo|null, statusEffects: string[] }[], element: string, level: number, maxVassals: number): number
{
  if(vassals.length == 0)
    return 0;

  const totalVassalLevel = vassals.reduce((p, v) => {
    const effectiveLevel =
      v.level +
      weaponBonusHuntingLevels(v.weapon) +
      (v.statusEffects.indexOf('Power') >= 0 ? 5 : 0)
    ;

    if(elementIsStrongAgainst(v.element, element))
      return p + Math.trunc(effectiveLevel * 3 / 2) + 1;

    if(elementIsWeakAgainst(v.element, element))
      return p + Math.trunc(effectiveLevel * 1 / 2) + 1;

    return p + effectiveLevel + 1;
  }, 0);

  const fraction = totalVassalLevel / (level + maxVassals);

  return fractionAsPercent(fraction);
}

export function computeAttackDamage(vassals: { level: number, element: string, weapon: WeaponInfo|null, statusEffects: string[] }[], element: string): number
{
  if(vassals.length == 0)
    return 0;

  return vassals.reduce((p, v) => {
    const effectiveLevel =
      v.level +
      weaponBonusHuntingLevels(v.weapon) +
      (v.statusEffects.indexOf('Power') >= 0 ? 5 : 0)
    ;

    const base = Math.trunc((effectiveLevel + 5) * (effectiveLevel + 5) / 10);

    if(elementIsStrongAgainst(v.element, element))
      return p + Math.trunc(base * 3 / 2);

    if(elementIsWeakAgainst(v.element, element))
      return p + Math.trunc(base / 2);

    return p + base;
  }, 0);
}

export const elementsDefeatedBy: {[key: string]: string[]} = {
  Earth: [ 'Lightning' ],
  Fire: [ 'Plant', 'Ice', 'Metal' ],
  Water: [ 'Fire' ],
  Plant: [ 'Water', 'Earth' ],
  Ice: [ 'Plant', 'Water' ],
  Lightning: [ 'Plant', 'Water', 'Metal' ],
  Metal: [ 'Plant', 'Earth' ]
};

export function elementIsStrongAgainst(attacker: string, defender: string)
{
  return elementsDefeatedBy[attacker].includes(defender);
}

export function elementIsWeakAgainst(attacker: string, defender: string)
{
  return elementsDefeatedBy[defender].includes(attacker);
}
