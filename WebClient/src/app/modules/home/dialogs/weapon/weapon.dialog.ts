import {Component, Inject, OnInit} from '@angular/core';
import {MAT_DIALOG_DATA, MatDialog, MatDialogRef} from "@angular/material/dialog";
import {ResourceQuantityDto} from "../../../../dtos/resource-quantity.dto";
import { ApiService } from "../../../base/services/api.service";
import { YieldOrientation } from "../../../game-common/components/describe-yield/describe-yield.component";
import {RepairWeaponDialog} from "../repair-weapon/repair-weapon.dialog";
import {YesOrNoDialog} from "../../../base/dialogs/yes-or-no/yes-or-no.dialog";
import {Observable, of, Subject} from "rxjs";
import {TownResourcesService} from "../../../base/services/town-resources.service";

@Component({
  templateUrl: './weapon.dialog.html',
  styleUrls: ['./weapon.dialog.scss']
})
export class WeaponDialog implements OnInit {

  YieldOrientation = YieldOrientation;

  waitingOnAjax = false;

  id: string;
  name: string;
  level: number;
  primaryBonus: string;
  secondaryBonus: string|null;
  durability: number;
  maxDurability: number;
  cost: ResourceQuantityDto[]|null;
  equippedToVassalId: string|null;
  hasScrappingI: boolean;

  constructor(
    @Inject(MAT_DIALOG_DATA) data: any, private dialogRef: MatDialogRef<WeaponDialog>,
    private api: ApiService, private matDialog: MatDialog, private town: TownResourcesService
  ) {
    this.id = data.id;
    this.name = data.name;
    this.primaryBonus = data.primaryBonus;
    this.secondaryBonus = data.secondaryBonus;
    this.durability = data.durability;
    this.maxDurability = data.maxDurability;
    this.level = data.level;
    this.cost = data.cost;
    this.equippedToVassalId = data.vassalId;
    this.hasScrappingI = data.hasScrappingI;
  }

  ngOnInit(): void {
  }

  doLevelUp()
  {
    if(this.waitingOnAjax || !this.cost)
      return;

    this.waitingOnAjax = true;
    this.dialogRef.disableClose = true;

    this.api.post<UpdatedWeapon>('weapons/' + this.id + '/levelUp').subscribe({
      next: r => {
        this.dialogRef.close(r.data);
      },
      error: () => {
        this.dialogRef.disableClose = false;
        this.waitingOnAjax = false;
      }
    })
  }

  doRepair()
  {
    if(this.waitingOnAjax)
      return;

    RepairWeaponDialog.open(this.matDialog, this.id, this.primaryBonus).afterClosed().subscribe({
      next: repaired => {
        if(repaired)
          this.dialogRef.close(true);
      }
    });
  }

  doScrap()
  {
    if(this.waitingOnAjax)
      return;

    YesOrNoDialog.open(this.matDialog, () => this.reallyScrapWeapon(), 'Scrap It?', 'You\'ll receive either 50 Wood, 25 Iron, or 10 Quintessence, at random.');
  }

  reallyScrapWeapon(): Observable<boolean>
  {
    if(this.waitingOnAjax) return of(false);

    const result = new Subject<boolean>();

    this.waitingOnAjax = true;

    this.api.post('weapons/' + this.id + '/scrap').subscribe({
      next: () => {
        result.next(true);
        result.complete();
        this.dialogRef.close(true);
        this.town.reload();
      },
      error: () => {
        this.waitingOnAjax = false;
        result.next(false);
        result.complete();
      }
    });

    return result;
  }

  doUnequip()
  {
    if(this.waitingOnAjax || !this.equippedToVassalId)
      return;

    this.waitingOnAjax = true;
    this.dialogRef.disableClose = true;

    this.api.post('vassals/' + this.equippedToVassalId + '/unequip').subscribe({
      next: _ => {
        this.dialogRef.close(true);
      },
      error: () => {
        this.dialogRef.disableClose = false;
        this.waitingOnAjax = false;
      }
    });
  }

  doCancel()
  {
    this.dialogRef.close();
  }

  static open(
    matDialog: MatDialog,
    id: string,
    name: string,
    level: number,
    primaryBonus: string,
    secondaryBonus: string|null,
    durability: number,
    maxDurability: number,
    cost: ResourceQuantityDto[]|null,
    vassalId: string|null|undefined,
    hasScrappingI: boolean
  ): MatDialogRef<WeaponDialog>
  {
    return matDialog.open(WeaponDialog, {
      data: {
        id: id,
        name: name,
        primaryBonus: primaryBonus,
        secondaryBonus: secondaryBonus,
        level: level,
        cost: cost,
        durability: durability,
        maxDurability: maxDurability,
        vassalId: vassalId,
        hasScrappingI: hasScrappingI
      }
    })
  }
}

interface UpdatedWeapon
{
  level: number;
  primaryEffect: string;
  secondaryEffect: string|null;
  resourcesToLevelUp: ResourceQuantityDto[]|null;
}
