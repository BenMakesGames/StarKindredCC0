import { Component, Inject, OnInit } from '@angular/core';
import { MAT_DIALOG_DATA, MatDialog, MatDialogRef } from "@angular/material/dialog";
import { BuildingDto } from "../../../../dtos/building.dto";
import { ApiService } from "../../../base/services/api.service";
import {YieldOrientation} from "../../../game-common/components/describe-yield/describe-yield.component";
import {RebuildDialog} from "../rebuild/rebuild.dialog";

@Component({
  templateUrl: './specialize.dialog.html',
  styleUrls: ['./specialize.dialog.scss']
})
export class SpecializeDialog implements OnInit {

  YieldOrientation = YieldOrientation;

  upgrading = false;

  buildingId: string;
  availableSpecializations: string[];

  selection: string|null = null;

  constructor(
    @Inject(MAT_DIALOG_DATA) data: any,
    private matDialog: MatDialog,
    private dialogRef: MatDialogRef<SpecializeDialog>,
    private api: ApiService
  ) {
    this.buildingId = data.buildingId;
    this.availableSpecializations = data.availableSpecializations;
  }

  ngOnInit(): void {
  }

  doUpgrade()
  {
    if(this.upgrading)
      return;

    this.upgrading = true;
    this.dialogRef.disableClose = true;

    this.api.post('buildings/' + this.buildingId + '/specialize', { type: this.selection }).subscribe({
      next: _ => {
        this.dialogRef.close(this.selection);
      },
      error: () => {
        this.upgrading = false;
        this.dialogRef.disableClose = false;
      }
    });
  }

  doCancel()
  {
    this.dialogRef.close();
  }

  doRebuild()
  {
    RebuildDialog.open(this.matDialog, this.buildingId, 10).afterClosed().subscribe({
      next: rebuilt => {
        if(rebuilt)
          this.dialogRef.close(true);
      }
    });
  }

  static open(matDialog: MatDialog, building: BuildingDto): MatDialogRef<SpecializeDialog>
  {
    return matDialog.open(SpecializeDialog, {
      data: {
        buildingId: building.id,
        availableSpecializations: building.availableSpecializations,
      }
    });
  }
}
