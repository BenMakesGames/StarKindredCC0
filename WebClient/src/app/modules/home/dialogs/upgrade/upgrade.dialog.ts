import { Component, Inject, OnInit } from '@angular/core';
import { MAT_DIALOG_DATA, MatDialog, MatDialogRef } from "@angular/material/dialog";
import { BuildingDto } from "../../../../dtos/building.dto";
import { ApiService } from "../../../base/services/api.service";
import { YieldOrientation } from '../../../game-common/components/describe-yield/describe-yield.component';
import { ResourceQuantityDto } from "../../../../dtos/resource-quantity.dto";
import {RebuildDialog} from "../rebuild/rebuild.dialog";

@Component({
  templateUrl: './upgrade.dialog.html',
  styleUrls: ['./upgrade.dialog.scss']
})
export class UpgradeDialog implements OnInit {

  YieldOrientation = YieldOrientation;

  upgrading = false;

  buildingId: string
  type: string;
  level: number;
  maxLevel: number;
  cost: ResourceQuantityDto[];

  constructor(
    @Inject(MAT_DIALOG_DATA) data: any,
    private matDialog: MatDialog,
    private dialogRef: MatDialogRef<UpgradeDialog>,
    private api: ApiService
  ) {
    this.buildingId = data.buildingId;
    this.type = data.type;
    this.level = data.level;
    this.maxLevel = data.maxLevel;
    this.cost = data.cost;
  }

  ngOnInit(): void {
  }

  doUpgrade()
  {
    if(this.upgrading)
      return;

    this.upgrading = true;
    this.dialogRef.disableClose = true;

    this.api.post('buildings/' + this.buildingId + '/upgrade').subscribe({
      next: _ => {
        this.dialogRef.close(true);
      },
      error: () => {
        this.upgrading = false;
        this.dialogRef.disableClose = false;
      }
    });
  }

  doRebuild()
  {
    RebuildDialog.open(this.matDialog, this.buildingId, this.level).afterClosed().subscribe({
      next: rebuilt => {
        if(rebuilt)
          this.dialogRef.close(true);
      }
    });
  }

  doCancel()
  {
    this.dialogRef.close();
  }

  static open(matDialog: MatDialog, building: BuildingDto): MatDialogRef<UpgradeDialog>
  {
    return matDialog.open(UpgradeDialog, {
      data: {
        buildingId: building.id,
        type: building.type,
        level: building.level,
        maxLevel: building.maxLevel,
        cost: building.upgradeCost,
      }
    });
  }
}
