import {Component, Inject, OnInit} from '@angular/core';
import { ApiService } from "../../../base/services/api.service";
import { ResourceQuantityDto } from "../../../../dtos/resource-quantity.dto";
import {MAT_DIALOG_DATA, MatDialog, MatDialogRef} from "@angular/material/dialog";

@Component({
  templateUrl: './equip-weapon.dialog.html',
  styleUrls: ['./equip-weapon.dialog.scss']
})
export class EquipWeaponDialog implements OnInit {

  equipping = false;
  weapons: WeaponDto[]|null = null;
  selected: string|null = null;
  vassalId: string;

  constructor(
    private api: ApiService, private dialogRef: MatDialogRef<EquipWeaponDialog>,
    @Inject(MAT_DIALOG_DATA) data: any
  )
  {
    this.vassalId = data.vassalId;
  }

  ngOnInit(): void {
    this.api.get<{ weapons: WeaponDto[] }>('treasures/my').subscribe({
      next: r => {
        this.weapons = r.data.weapons.sort((a, b) => {
          const imageComparison = a.primaryEffect.localeCompare(b.primaryEffect);

          if (imageComparison != 0)
            return imageComparison;

          return b.level - a.level;
        });
      }
    });
  }

  doClose()
  {
    this.dialogRef.close();
  }

  doEquip()
  {
    if(this.selected == null || this.equipping)
      return;

    this.equipping = true;
    this.dialogRef.disableClose = true;

    this.api.post('vassals/' + this.vassalId + '/equip', { weaponId: this.selected }).subscribe({
      next: _ => {
        this.dialogRef.close(true);
      },
      error: () => {
        this.equipping = false;
        this.dialogRef.disableClose = false;
      }
    });
  }

  static open(matDialog: MatDialog, vassalId: string): MatDialogRef<EquipWeaponDialog>
  {
    return matDialog.open(EquipWeaponDialog, {
      data: {
        vassalId: vassalId
      }
    });
  }
}

interface WeaponDto
{
  id: string;
  name: string;
  level: number;
  image: string;
  primaryEffect: string;
  secondaryEffect: string|null;
  resourcesToLevelUp: ResourceQuantityDto[]|null;
  vassal: { id: string, name: string, species: string, portrait: string, element: string, level: number }|null;
}
