import {Component, Inject, OnInit} from '@angular/core';
import {MAT_DIALOG_DATA, MatDialog, MatDialogRef} from "@angular/material/dialog";
import {ResourceQuantityDto} from "../../../../dtos/resource-quantity.dto";
import { ApiService } from "../../../base/services/api.service";
import { YieldOrientation } from "../../../game-common/components/describe-yield/describe-yield.component";

@Component({
  templateUrl: './level-up.dialog.html',
  styleUrls: ['./level-up.dialog.scss']
})
export class LevelUpDialog implements OnInit {

  YieldOrientation = YieldOrientation;

  leveling = false;

  id!: string;
  name!: string;
  level!: number;
  cost!: ResourceQuantityDto[];

  constructor(
    @Inject(MAT_DIALOG_DATA) data: any, private dialogRef: MatDialogRef<LevelUpDialog>,
    private api: ApiService
  ) {
    this.id = data.id;
    this.name = data.name;
    this.level = data.level;
    this.cost = data.cost;
  }

  ngOnInit(): void {
  }

  doLevelUp()
  {
    if(this.leveling)
      return;

    this.leveling = true;
    this.dialogRef.disableClose = true;

    this.api.post('vassals/' + this.id + '/levelUp').subscribe({
      next: _ => {
        this.dialogRef.close(true);
      },
      error: () => {
        this.dialogRef.disableClose = false;
        this.leveling = false;
      }
    })
  }

  doCancel()
  {
    this.dialogRef.close();
  }

  static open(matDialog: MatDialog, id: string, name: string, level: number, cost: ResourceQuantityDto[]): MatDialogRef<LevelUpDialog>
  {
    return matDialog.open(LevelUpDialog, {
      data: {
        id: id,
        name: name,
        level: level,
        cost: cost
      }
    })
  }
}
