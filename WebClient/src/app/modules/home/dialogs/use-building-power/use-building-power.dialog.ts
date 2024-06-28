import {Component, Inject} from '@angular/core';
import {MAT_DIALOG_DATA, MatDialog, MatDialogRef} from "@angular/material/dialog";
import {ApiService} from "../../../base/services/api.service";
import {ResourceQuantityDto} from "../../../../dtos/resource-quantity.dto";
import {BuildingDto, PowerDto} from "../../../../dtos/building.dto";
import {TownResourcesService} from "../../../base/services/town-resources.service";

@Component({
  templateUrl: './use-building-power.dialog.html',
  styleUrls: ['./use-building-power.dialog.scss']
})
export class UseBuildingPowerDialog {

  opening = false;
  choice: string|undefined;
  selectedCost: ResourceQuantityDto|null = null;

  building: BuildingDto;

  constructor(
    @Inject(MAT_DIALOG_DATA) data: any,
    private dialogRef: MatDialogRef<UseBuildingPowerDialog>,
    private api: ApiService,
    private town: TownResourcesService
  ) {
    this.building = data.building;
  }

  doSelect(power: PowerDto)
  {
    this.choice = power.power;
    this.selectedCost = power.cost;
  }

  doUsePower()
  {
    if(this.opening)
      return;

    this.opening = true;
    this.dialogRef.disableClose = true;

    const data = { choice: this.choice };

    this.api.post<Response>('buildings/' + this.building.id + '/activatePower', data).subscribe({
      next: r => {

        if(this.selectedCost)
          this.town.spend([ this.selectedCost ]);

        if(r.data.resources)
          this.town.earn([ r.data.resources ]);

        this.dialogRef.close(r.data);
      },
      error: () => {
        this.opening = false;
        this.dialogRef.disableClose = false;
      }
    });
  }

  doClose()
  {
    this.dialogRef.close();
  }

  static open(matDialog: MatDialog, building: BuildingDto): MatDialogRef<UseBuildingPowerDialog>
  {
    return matDialog.open(UseBuildingPowerDialog, {
      data: {
        building: building
      }
    });
  }
}

export interface Response
{
  powersAvailableOn: string;
  resources: ResourceQuantityDto;
  unlockedDecorations: boolean;
}
