import { Component, Inject } from '@angular/core';
import { ApiService } from "../../../base/services/api.service";
import { MAT_DIALOG_DATA, MatDialog, MatDialogRef } from "@angular/material/dialog";
import {VassalSearchResultModel} from "../../../../dtos/vassal-search-result.model";

@Component({
  templateUrl: './go-on-generic-hunt-mission.dialog.html',
  styleUrls: ['./go-on-generic-hunt-mission.dialog.scss']
})
export class GoOnGenericHuntMissionDialog {

  embarking = false;

  tags: { title: string, color: string }[];
  mission: string;
  level: number;
  maxVassals: number;

  selectedVassals: VassalSearchResultModel[] = [];

  constructor(
    @Inject(MAT_DIALOG_DATA) data: any,
    private dialogRef: MatDialogRef<GoOnGenericHuntMissionDialog>,
    private api: ApiService
  ) {
    this.tags = data.tags;
    this.mission = data.mission;
    this.level = data.level;
    this.maxVassals = data.maxVassals;
  }

  doGoOnMission()
  {
    if(this.embarking)
      return;

    this.embarking = true;
    this.dialogRef.disableClose = true;

    const data = {
      mission: this.mission,
      vassals: this.selectedVassals.map(v => v.id)
    }

    this.api.post('missions', data).subscribe({
      next: _ => {
        this.dialogRef.close(true);
      },
      error: () => {
        this.embarking = false;
        this.dialogRef.disableClose = false;
      }
    });
  }

  doCancel()
  {
    this.dialogRef.close();
  }

  static open(matDialog: MatDialog, tags: { title: string, color: string }[], mission: string, level: number, maxVassals: number): MatDialogRef<GoOnGenericHuntMissionDialog>
  {
    return matDialog.open(GoOnGenericHuntMissionDialog, {
      width: '100%',
      maxWidth: '640px',
      panelClass: 'big',
      data: {
        tags: tags,
        mission: mission,
        level: level,
        maxVassals: maxVassals,
      }
    });
  }
}
