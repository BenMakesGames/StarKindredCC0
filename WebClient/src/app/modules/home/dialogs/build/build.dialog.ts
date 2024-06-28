import { Component, Inject } from '@angular/core';
import { MAT_DIALOG_DATA, MatDialog, MatDialogRef } from "@angular/material/dialog";
import { ApiService } from "../../../base/services/api.service";
import {ResourceQuantityDto} from "../../../../dtos/resource-quantity.dto";
import {YieldOrientation} from "../../../game-common/components/describe-yield/describe-yield.component";

@Component({
  templateUrl: './build.dialog.html',
  styleUrls: ['./build.dialog.scss']
})
export class BuildDialog {

  YieldOrientation = YieldOrientation;

  waitingOnApi = false;
  position: number;
  resources: ResourceQuantityDto[];
  options: Option[]|null = null;
  selection: string|null = null;

  constructor(
    @Inject(MAT_DIALOG_DATA) private data: any,
    private dialogRef: MatDialogRef<BuildDialog>,
    private api: ApiService
  ) {
    this.position = data.position;
    this.resources = data.resources;

    this.api.get<BuildOptionsDto>('buildings/canBuild', { position: this.position }).subscribe({
      next: r => {
        this.options = r.data.options.map(o => {
          return {
            ...o,
            canAfford: this.canAfford(o.cost)
          }
        });
      }
    });
  }

  canAfford(cost: ResourceQuantityDto[]): boolean
  {
    return cost.every(c =>
      (this.resources.find(r => r.type === c.type)?.quantity ?? 0) >= c.quantity
    );
  }

  doBuild()
  {
    if(this.waitingOnApi)
      return;

    this.waitingOnApi = true;
    this.dialogRef.disableClose = true;

    this.api.post('buildings/build', { position: this.position, building: this.selection }).subscribe({
      next: _ => {
        this.dialogRef.close(true);
      },
      error: () => {
        this.waitingOnApi = false;
        this.dialogRef.disableClose = false;
      }
    })
  }

  doCancel()
  {
    this.dialogRef.close();
  }

  static open(matDialog: MatDialog, position: number, resources: ResourceQuantityDto[]): MatDialogRef<BuildDialog>
  {
    return matDialog.open(BuildDialog, {
      data: {
        position: position,
        resources: resources
      }
    });
  }
}

interface BuildOptionsDto
{
  options: Option[];
}

interface Option
{
  building: string;
  canAfford: boolean;
  cost: ResourceQuantityDto[];
}
