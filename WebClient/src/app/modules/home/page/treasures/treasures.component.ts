import { Component, OnDestroy, OnInit } from '@angular/core';
import { ApiService } from "../../../base/services/api.service";
import { MatDialog } from "@angular/material/dialog";
import { HelpDialog } from "../../../game-common/dialogs/help/help.dialog";
import { WeaponDialog } from "../../dialogs/weapon/weapon.dialog";
import { ResourceQuantityDto } from "../../../../dtos/resource-quantity.dto";
import { TownResourcesService } from "../../../base/services/town-resources.service";
import { Subscription } from "rxjs";
import {OpenTreasureChestDialog, TreasureChoice} from "../../dialogs/open-treasure-chest/open-treasure-chest.dialog";

@Component({
  selector: 'app-treasures',
  templateUrl: './treasures.component.html',
  styleUrls: ['./treasures.component.scss']
})
export class TreasuresComponent implements OnInit, OnDestroy {

  resourcesSubscription = Subscription.EMPTY;
  resources: ResourceQuantityDto[]|null = null;
  items: MyTreasure|null = null;

  constructor(private api: ApiService, private town: TownResourcesService, private matDialog: MatDialog) { }

  ngOnInit(): void {
    this.resourcesSubscription = this.town.resources.subscribe({
      next: r => {
        if(r)
          this.resources = r;
        else
          this.town.reload();
      }
    });

    this.loadTreasure();
  }

  ngOnDestroy()
  {
    this.resourcesSubscription.unsubscribe();
  }

  loadTreasure()
  {
    this.api.get<MyTreasure>('treasures/my').subscribe({
      next: r => {
        this.items = r.data;

        this.items.weapons = this.items.weapons.sort((a, b) => {
          const imageComparison = a.primaryEffect.localeCompare(b.primaryEffect);

          if (imageComparison != 0)
            return imageComparison;

          return b.level - a.level;
        });
      }
    });
  }

  doInteractWithTreasure(t: Treasure)
  {
    if(t.type in TreasuresComponent.treasureChoices)
    {
      if(t.quantity == 0)
        return;

      const data = {
        ...TreasuresComponent.treasureChoices[t.type],
        choices: TreasuresComponent.treasureChoices[t.type].choices.filter(c => {
            if(!c.techRequirement)
              return true;

            return this.items && this.items.technologies.indexOf(c.techRequirement) >= 0;
        })
      };

      OpenTreasureChestDialog.open(this.matDialog, data.endPoint, data.choices, data.allowMultiple ? t.quantity : 1, data.useLabel).afterClosed().subscribe({
        next: opened => {
          if(opened)
          {
            this.loadTreasure();
            this.town.reload();
          }
        }
      })
    }
    else
      HelpDialog.open(this.matDialog, 'treasure-' + t.type.toLowerCase())
  }

  private static treasureChoices: { [key: string]: { endPoint: string, choices: TreasureChoice[], allowMultiple: boolean, useLabel: string } } = {
    TreasureMap: {
      endPoint: 'treasureMap',
      choices: [
        { label: 'Track Treasure', value: null, image: 'ui/missions-square' }
      ],
      allowMultiple: false,
      useLabel: 'Read'
    },
    WrappedSword: {
      endPoint: 'wrappedSword',
      choices: [
        { label: 'Level 3 Sword', value: null, image: 'weapons/unknown/sword' }
      ],
      allowMultiple: false,
      useLabel: 'Unwrap'
    },
    CupOfLife: {
      endPoint: 'cupOfLife',
      choices: [
        { label: '500 Meat', value: 'Meat', image: 'resources/meat' },
        { label: '200 Quintessence', value: 'Quintessence', image: 'resources/quintessence' },
        { label: 'Ichor', value: 'Ichor', image: 'treasures/ichor' }
      ],
      allowMultiple: true,
      useLabel: 'Drink'
    },
    FishBag: {
      endPoint: 'fishBag',
      choices: [
        { label: '250 Meat', value: 'Meat', image: 'resources/meat' },
        { label: '100 Quintessence', value: 'Quintessence', image: 'resources/quintessence' },
      ],
      allowMultiple: true,
      useLabel: 'Open'
    },
    Soma: {
      endPoint: 'soma',
      choices: [
        { label: '1000 Wheat', value: 'Wheat', image: 'resources/wheat' },
        { label: '400 Wine', value: 'Wine', image: 'resources/wine' },
        { label: '200 Quintessence', value: 'Quintessence', image: 'resources/quintessence' },
      ],
      allowMultiple: true,
      useLabel: 'Drink'
    },
    GoldChest: {
      endPoint: 'goldChest',
      choices: [
        { label: '300 Marble', value: 'Marble', image: 'resources/marble' },
        { label: '300 Gold', value: 'Gold', image: 'resources/gold' },
        { label: 'Magic Hammer', value: 'MagicHammer', image: 'treasures/magichammer' }
      ],
      allowMultiple: true,
      useLabel: 'Open'
    },
    BoxOfOres: {
      endPoint: 'boxOfOres',
      choices: [
        { label: '100 Stone', value: 'Stone', image: 'resources/stone' },
        { label: '75 Iron', value: 'Iron', image: 'resources/iron' },
        { label: '50 Marble', value: 'Marble', image: 'resources/marble' },
        { label: '50 Gold', value: 'Gold', image: 'resources/gold' },
      ],
      allowMultiple: true,
      useLabel: 'Open'
    },
    BasicChest: {
      endPoint: 'basicChest',
      choices: [
        { label: '200 Wheat', value: 'Wheat', image: 'resources/wheat' },
        { label: '150 Wood', value: 'Wood', image: 'resources/wood' },
        { label: '100 Meat', value: 'Meat', image: 'resources/meat' },
        { label: '50 Gold', value: 'Gold', image: 'resources/gold' },
      ],
      allowMultiple: true,
      useLabel: 'Open'
    },
    BigBasicChest: {
      endPoint: 'bigBasicChest',
      choices: [
        { label: '400 Wheat', value: 'Wheat', image: 'resources/wheat' },
        { label: '300 Wood', value: 'Wood', image: 'resources/wood' },
        { label: '200 Meat', value: 'Meat', image: 'resources/meat' },
        { label: '100 Gold', value: 'Gold', image: 'resources/gold' },
      ],
      allowMultiple: true,
      useLabel: 'Open'
    },
    RubyChest: {
      endPoint: 'rubyChest',
      choices: [
        { label: '300 Iron', value: 'Iron', image: 'resources/iron' },
        { label: '200 Quintessence', value: 'Quintessence', image: 'resources/quintessence' },
        { label: 'Ichor', value: 'Ichor', image: 'treasures/ichor' }
      ],
      allowMultiple: true,
      useLabel: 'Open'
    },
    TwilightChest: {
      endPoint: 'twilightChest',
      choices: [
        { label: '200 Gold', value: 'Gold', image: 'resources/gold' },
        { label: '150 Quintessence', value: 'Quintessence', image: 'resources/quintessence' },
        { label: 'Renaming Scroll', value: 'RenamingScroll', image: 'treasures/renamingscroll' },
      ],
      allowMultiple: true,
      useLabel: 'Open'
    },
    WeaponChest: {
      endPoint: 'weaponChest',
      choices: [
        { label: 'Sword', value: 'HuntingLevels', image: 'weapons/unknown/sword'},
        { label: 'Horn', value: 'FasterMissions', image: 'weapons/unknown/horn' },
        { label: 'Shovel', value: 'MoreGold', image: 'weapons/unknown/shovel' },
        { label: 'Axe', value: 'MeatGetsWood', image: 'weapons/unknown/axe' },
        { label: 'Wand', value: 'GoldGetsWine', image: 'weapons/unknown/wand' },
        { label: 'Scythe', value: 'WeaponsGetWheat', image: 'weapons/unknown/scythe' },
        { label: 'Lyre', value: 'RecruitBonus', image: 'weapons/unknown/lyre' },
        { label: '50 Wood', value: 'Wood', image: 'resources/wood', techRequirement: 'ScrappingII' },
        { label: '25 Iron', value: 'Iron', image: 'resources/iron', techRequirement: 'ScrappingII' },
        { label: '10 Quint', value: 'Quint', image: 'resources/quintessence', techRequirement: 'ScrappingII' },
      ],
      allowMultiple: true,
      useLabel: 'Open'
    },
    Emerald: {
      endPoint: 'emerald',
      choices: [
        { label: '300 Wine', value: 'Wine', image: 'resources/wine' },
        { label: '200 Gold', value: 'Gold', image: 'resources/gold' },
        { label: '150 Quintessence', value: 'Quintessence', image: 'resources/quintessence' },
        { label: 'Wand', value: 'Wand', image: 'weapons/unknown/wand' },
      ],
      allowMultiple: false,
      useLabel: 'Trade'
    },
    CrystallizedQuint: {
      endPoint: 'crystallizedQuint',
      choices: [
        { label: '1500 Gold', value: 'Gold', image: 'resources/gold' },
        { label: '1000 Quintessence', value: 'Quintessence', image: 'resources/quintessence' },
      ],
      allowMultiple: true,
      useLabel: 'Use'
    },
    RallyingStandard: {
      endPoint: 'rallyingStandard',
      choices: [
        { label: 'Second Attack on Giant', value: null, image: 'treasures/rallyingstandard' }
      ],
      allowMultiple: false,
      useLabel: 'Rally'
    },
  };

  doShowWeapon(w: Weapon)
  {
    const hasScrappingI = !!this.items && this.items.technologies.indexOf('ScrappingI') >= 0;

    WeaponDialog.open(this.matDialog, w.id, w.name, w.level, w.primaryEffect, w.secondaryEffect, w.durability, w.maxDurability, w.resourcesToLevelUp, w.vassal?.id, hasScrappingI)
      .afterClosed()
      .subscribe({
        next: r => {
          if(!r)
            return;

          if(r === true)
          {
            this.loadTreasure();
            return;
          }

          this.items = {
            ...this.items!,
            weapons: this.items!.weapons.map(weapon => {
              if(weapon.id != w.id)
                return weapon;

              return {
                ...weapon,
                level: r.level,
                primaryEffect: r.primaryEffect,
                secondaryEffect: r.secondaryEffect,
                resourcesToLevelUp: r.resourcesToLevelUp,
              };
            })
          };

          this.town.reload();
        }
      });
  }
}

interface MyTreasure
{
  treasures: Treasure[];
  weapons: Weapon[];
  technologies: string[];
}

interface Treasure
{
  type: string;
  quantity: number;
}

interface Weapon
{
  id: string;
  name: string;
  image: string;
  level: number;
  primaryEffect: string;
  secondaryEffect: string|null;
  resourcesToLevelUp: ResourceQuantityDto[]|null;
  durability: number;
  maxDurability: number;
  vassal: { id: string, name: string, level: number, element: string, species: string, portrait: string}|null;
}
