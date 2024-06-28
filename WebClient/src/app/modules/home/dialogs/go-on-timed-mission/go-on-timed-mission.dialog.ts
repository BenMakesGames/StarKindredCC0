import { Component, Inject } from '@angular/core';
import { ApiService } from "../../../base/services/api.service";
import { MAT_DIALOG_DATA, MatDialog, MatDialogRef } from "@angular/material/dialog";
import {VassalSearchResultModel} from "../../../../dtos/vassal-search-result.model";

@Component({
  templateUrl: './go-on-timed-mission.dialog.html',
  styleUrls: ['./go-on-timed-mission.dialog.scss']
})
export class GoOnTimedMissionDialog {

  embarking = false;

  mission: string;
  missionId: string;
  level: number;
  element: string|null;
  minVassals: number;
  maxVassals: number;
  applyHuntingBonus: boolean;
  tags: { title: string, color: string }[];

  selectedVassals: VassalSearchResultModel[] = [];

  constructor(
    @Inject(MAT_DIALOG_DATA) data: any,
    private dialogRef: MatDialogRef<GoOnTimedMissionDialog>,
    private api: ApiService
  ) {
    this.tags = data.tags;
    this.mission = data.mission;
    this.missionId = data.missionId;
    this.level = data.level;
    this.element = data.element;
    this.minVassals = data.minVassals;
    this.maxVassals = data.maxVassals;
    this.applyHuntingBonus = this.mission == 'WanderingMonster';
  }

  doGoOnMission()
  {
    if(this.embarking)
      return;

    this.embarking = true;
    this.dialogRef.disableClose = true;

    const data = {
      id: this.missionId,
      vassals: this.selectedVassals.map(v => v.id)
    }

    this.api.post('timedMissions', data).subscribe({
      next: _ => {
        this.dialogRef.close(true);
      },
      error: () => {
        this.embarking = false;
        this.dialogRef.disableClose = false;
      }
    })
  }

  doCancel()
  {
    this.dialogRef.close();
  }

  static open(matDialog: MatDialog, tags: { title: string, color: string }[], mission: string, missionId: string, level: number, element: string|null, minVassals: number, maxVassals: number): MatDialogRef<GoOnTimedMissionDialog>
  {
    return matDialog.open(GoOnTimedMissionDialog, {
      width: '100%',
      maxWidth: '640px',
      panelClass: 'big',
      data: {
        tags: tags,
        mission: mission,
        missionId: missionId,
        level: level,
        element: element,
        minVassals: minVassals,
        maxVassals: maxVassals,
      }
    });
  }
}
