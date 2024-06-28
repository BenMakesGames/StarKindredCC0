import {Component, Inject, OnInit} from '@angular/core';
import {MAT_DIALOG_DATA, MatDialog, MatDialogRef} from "@angular/material/dialog";
import {ApiService} from "../../../base/services/api.service";
import {PaginatedResultsDto} from "../../../../dtos/paginated-results.dto";

@Component({
  templateUrl: './repair-weapon.dialog.html',
  styleUrls: ['./repair-weapon.dialog.scss']
})
export class RepairWeaponDialog implements OnInit {

  repairing = false;
  selectedWeaponId: string|null = null;
  weapons: PaginatedResultsDto<WeaponDto>|null = null;
  weaponId: string;
  primaryBonus: string;

  constructor(
    @Inject(MAT_DIALOG_DATA) data: any,
    private dialogRef: MatDialogRef<RepairWeaponDialog>,
    private api: ApiService
  ) {
    this.weaponId = data.weaponId;
    this.primaryBonus = data.primaryBonus;
  }

  ngOnInit(): void {
    this.api.get<PaginatedResultsDto<WeaponDto>>('weapons', { primaryBonus: this.primaryBonus, excludedWeapons: [ this.weaponId ] }).subscribe({
      next: r => {
        this.weapons = r.data;
      }
    });
  }

  doCancel()
  {
    this.dialogRef.close();
  }

  doRepair()
  {
    if(this.repairing || !this.selectedWeaponId)
      return;

    this.repairing = true;
    this.dialogRef.disableClose = true;

    this.api.post('weapons/' + this.weaponId + '/repair', { materialsId: this.selectedWeaponId }).subscribe({
      next: _ => {
        this.dialogRef.close(true);
      },
      error: () => {
        this.repairing = false;
        this.dialogRef.disableClose = false;
      }
    });
  }

  public static open(matDialog: MatDialog, weaponId: string, primaryBonus: string): MatDialogRef<RepairWeaponDialog>
  {
    return matDialog.open(RepairWeaponDialog, {
      data: {
        weaponId: weaponId,
        primaryBonus: primaryBonus
      }
    });
  }
}

interface WeaponDto
{
  id: string;
  name: string;
  image: string;
  level: number;
  primaryEffect: string;
  secondaryEffect: string|null;
  durability: number;
  maxDurability: number;
  repairValue: number;
  vassal: { id: string, name: string, level: number, element: string, species: string, portrait: string}|null;
}
