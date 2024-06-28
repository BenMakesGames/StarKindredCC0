import {Component, OnDestroy, OnInit} from '@angular/core';
import {ApiService} from "../../../base/services/api.service";
import {ActivatedRoute, Router} from "@angular/router";
import {MessagesService} from "../../../base/services/messages.service";
import {Subscription} from "rxjs";
import {ResourceQuantityDto} from "../../../../dtos/resource-quantity.dto";
import { LevelUpDialog } from "../../dialogs/level-up/level-up.dialog";
import { MatDialog } from "@angular/material/dialog";
import {StatusEffectDialog} from "../../dialogs/status-effect/status-effect.dialog";
import { RenameDialog } from "../../dialogs/rename/rename.dialog";
import { TownResourcesService } from "../../../base/services/town-resources.service";
import { EquipWeaponDialog } from "../../dialogs/equip-weapon/equip-weapon.dialog";
import { WeaponDialog } from "../../dialogs/weapon/weapon.dialog";
import {SpendWillpowerDialog} from "../../dialogs/spend-willpower/spend-willpower.dialog";
import {AddTagDialog} from "../../dialogs/add-tag/add-tag.dialog";
import {RetireVassalDialog} from "../../dialogs/retire-vassal/retire-vassal.dialog";
import {Vassal} from "../../../game-common/components/vassal-portrait/vassal-portrait.component";
import {DismissVassalDialog} from "../../dialogs/dismiss-vassal/dismiss-vassal.dialog";

@Component({
  templateUrl: './vassal-details.component.html',
  styleUrls: ['./vassal-details.component.scss']
})
export class VassalDetailsComponent implements OnInit, OnDestroy {

  vassalId: string|null = null!;
  vassal: VassalDetailsDto|null = null;
  technologies: string[] = [];
  vassalPortraitInfo: Vassal|null = null;
  usefulTreasure: UsefulTreasureDto[] = [];
  loadingSubscription = Subscription.EMPTY;
  paramSubscription = Subscription.EMPTY;
  resourcesSubscription = Subscription.EMPTY;
  canLevel = false;
  maxLevel = 100;
  resources: ResourceQuantityDto[]|null = null;
  statusEffects: string[] = [];
  hasRenamingScrolls = false;
  removingTags = false;

  constructor(
    private api: ApiService, private activatedRoute: ActivatedRoute, private router: Router,
    private messages: MessagesService, private matDialog: MatDialog, private town: TownResourcesService,
  ) { }

  ngOnInit(): void {
    this.resourcesSubscription = this.town.resources.subscribe({
      next: r => {
        if(r)
          this.resources = r;
        else
          this.town.reload();
      }
    });

    this.paramSubscription = this.activatedRoute.paramMap.subscribe(params => {
      this.vassalId = params.get('id')!;

      this.loadVassal();
    });
  }

  doShowWeapon()
  {
    if(!this.vassal?.weapon)
      return;

    const weapon = this.vassal.weapon;
    const hasScrappingI = this.technologies.indexOf('ScrappingI') >= 0;

    WeaponDialog.open(this.matDialog, weapon.id, weapon.name, weapon.level, weapon.primaryBonus, weapon.secondaryBonus, weapon.durability, weapon.maxDurability, weapon.resourcesToLevelUp, this.vassalId, hasScrappingI)
      .afterClosed()
      .subscribe({
        next: r => {
          if(!r)
            return;

          this.loadVassal();

          this.town.reload();
        }
      });
  }

  ngOnDestroy() {
    this.paramSubscription.unsubscribe();
    this.loadingSubscription.unsubscribe();
    this.resourcesSubscription.unsubscribe();
  }

  doRemoveTag(tag: { title: string, color: string })
  {
    if(!this.vassal)
      return;

    this.vassal = {
      ...this.vassal,
      tags: this.vassal.tags = this.vassal.tags.filter(t => t.title != tag.title)
    };

    this.api.post('vassals/' + this.vassalId + '/tags/delete', { title: tag.title }).subscribe({
      next: () => { },
      error: e => {
        if(e.messages.some((m: string) => m.indexOf('doesn\'t have that tag')))
          return;

        if(!this.vassal)
          return;

        this.vassal.tags.push(tag);
        this.vassal = { ...this.vassal };
      }
    });
  }

  loadVassal()
  {
    this.loadingSubscription.unsubscribe();

    this.loadingSubscription = this.api.get<Response>('vassals/' + this.vassalId).subscribe({
      next: r => {
        this.technologies = r.data.technologies;
        this.vassal = r.data.vassal;
        this.maxLevel = r.data.vassal.sign === 'Mountain' ? 110 : 100;
        this.usefulTreasure = r.data.treasure;
        this.statusEffects = this.vassal.statusEffects.map(se => se.type);
        this.hasRenamingScrolls = this.usefulTreasure.some(t => t.type === 'RenamingScroll' && t.quantity > 0);

        this.vassalPortraitInfo = {
          ...this.vassal,
          onMission: !!this.vassal.mission,
          leader: !!this.vassal.leader,
          statusEffects: this.vassal.statusEffects.map(se => se.type),
        };
      },
      error: () => {
        this.noSuchVassal();
      }
    });
  }

  noSuchVassal()
  {
    this.messages.add({
      text: 'That vassal does not exist.',
      type: 'Error'
    })

    // noinspection JSIgnoredPromiseFromCall
    this.router.navigateByUrl('/home/vassals');
  }

  doSpendWillpower()
  {
    SpendWillpowerDialog.open(this.matDialog, this.vassalId!, this.vassal!.level, this.vassal!.willpower, this.vassal!.sign == 'LargeCupAndLittleCup' ? 4 : 3)
      .afterClosed()
      .subscribe({
        next: updated => {
          if(updated)
            this.loadVassal();
        }
      })
  }

  doEquipSomething()
  {
    EquipWeaponDialog.open(this.matDialog, this.vassalId!).afterClosed().subscribe({
      next: changed => {
        if(changed)
          this.loadVassal();
      }
    });
  }

  doRename()
  {
    const renamingScrollQuantity = this.usefulTreasure.find(t => t.type == 'RenamingScroll')?.quantity ?? 0;

    RenameDialog.open(this.matDialog, this.vassalId!, this.vassal!.name, renamingScrollQuantity).afterClosed().subscribe({
      next: refresh => {
        if(refresh)
          this.loadVassal();
      }
    });
  }

  doDismiss()
  {
    if(!this.vassal || this.vassal.favorite) return;

    DismissVassalDialog.open(this.matDialog, this.vassalId!, this.vassal.name, this.vassal.species, this.vassal.portrait, this.vassal.level, this.vassal.nature).afterClosed().subscribe({
      next: retired => {
        if(retired)
        {
          // noinspection JSIgnoredPromiseFromCall
          this.router.navigateByUrl('/home/vassals');
        }
      }
    });
  }

  doRetire()
  {
    if(!this.vassal || this.vassal.favorite) return;

    RetireVassalDialog.open(this.matDialog, this.vassalId!, this.vassal.name, this.vassal.species, this.vassal.portrait, this.vassal.level, this.vassal.nature).afterClosed().subscribe({
      next: retired => {
        if(retired)
        {
          // noinspection JSIgnoredPromiseFromCall
          this.router.navigateByUrl('/home/vassals');
        }
      }
    });
  }

  doShowStatusEffect(se: StatusEffectDto)
  {
    const ichorQuantity = this.usefulTreasure.find(t => t.type == 'Ichor')?.quantity ?? 0;

    StatusEffectDialog.open(this.matDialog, se.id, se.type, se.strength, ichorQuantity).afterClosed().subscribe({
      next: refresh => {
        if(refresh)
          this.loadVassal();
      }
    });
  }

  doLevelUp()
  {
    if(this.vassal == null)
      return;

    LevelUpDialog.open(this.matDialog, this.vassalId!, this.vassal.name, this.vassal.level, this.vassal.resourcesToLevelUp)
      .afterClosed()
      .subscribe({
        next: leveled => {
          if(!leveled)
            return;

          this.town.spend(this.vassal!.resourcesToLevelUp);

          this.loadVassal();
        }
      })
    ;
  }

  doRemoveTags()
  {
    this.removingTags = !this.removingTags;
  }

  doAddTag()
  {
    if(!this.vassalId)
      return;

    this.removingTags = false;

    AddTagDialog.open(this.matDialog, this.vassalId).afterClosed().subscribe({
      next: newTag => {
        if(!newTag)
          return;

        this.vassal = {
          ...this.vassal!,
          tags: [...this.vassal!.tags, newTag]
        };
      }
    });
  }
}

interface Response
{
  vassal: VassalDetailsDto;
  treasure: UsefulTreasureDto[];
  technologies: string[];
}

interface UsefulTreasureDto
{
  type: string;
  quantity: number;
}

interface VassalDetailsDto
{
  name: string;
  portrait: string;
  level: number;
  willpower: number;
  retirementProgress: number;
  favorite: boolean;
  species: string;
  element: string;
  nature: string;
  sign: string;
  recruitDate: string;
  tags: { title: string, color: string }[];
  statusEffects: StatusEffectDto[];
  resourcesToLevelUp: ResourceQuantityDto[];
  mission: string|null;
  leader: string|null;
  weapon: { id: string, name: string, image: string, level: number, primaryBonus: string, secondaryBonus: string, durability: number, maxDurability: number, resourcesToLevelUp: ResourceQuantityDto[]|null }|null;
  relationships: RelationshipDto[];
}

interface StatusEffectDto
{
  id: string;
  type: string;
  strength: number;
}

interface RelationshipDto
{
  id: string;
  name: string;
  portrait: string;
  species: string;
  element: string;
  level: number;
  favorite: boolean;
  relationshipMinutes: number;
  relationshipLevel: number;
  relationshipProgress: number;
}
