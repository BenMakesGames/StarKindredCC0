import { Component, Inject } from '@angular/core';
import { MAT_DIALOG_DATA, MatDialog, MatDialogRef } from "@angular/material/dialog";
import { ApiService } from "../../../base/services/api.service";
import {YieldOrientation} from "../../../game-common/components/describe-yield/describe-yield.component";

@Component({
  templateUrl: './rebuild.dialog.html',
  styleUrls: ['./rebuild.dialog.scss']
})
export class RebuildDialog {

  YieldOrientation = YieldOrientation;

  waitingOnApi = false;
  buildingId: string;
  currentLevel: number;
  newLevel: number|null = null;
  options: string[]|null = null;
  selection: string|null = null;

  constructor(
    @Inject(MAT_DIALOG_DATA) private data: any,
    private dialogRef: MatDialogRef<RebuildDialog>,
    private api: ApiService
  ) {
    this.buildingId = data.buildingId;
    this.currentLevel = data.level;

    this.api.get<RebuildOptionDto>('buildings/' + this.buildingId + '/rebuild').subscribe({
      next: r => {
        this.options = r.data.types;
        this.newLevel = r.data.newLevel;
      }
    });
  }

  doRebuild()
  {
    if(this.waitingOnApi)
      return;

    this.waitingOnApi = true;
    this.dialogRef.disableClose = true;

    this.api.post('buildings/' + this.buildingId + '/rebuild', { type: this.selection }).subscribe({
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

  static open(matDialog: MatDialog, buildingId: string, level: number): MatDialogRef<RebuildDialog>
  {
    return matDialog.open(RebuildDialog, {
      data: {
        buildingId: buildingId,
        level: level
      }
    });
  }
}

interface RebuildOptionDto
{
  types: string[];
  newLevel: number;
}
