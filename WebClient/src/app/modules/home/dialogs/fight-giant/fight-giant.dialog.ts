import { Component, Inject } from '@angular/core';
import { ApiService } from "../../../base/services/api.service";
import { MAT_DIALOG_DATA, MatDialog, MatDialogRef } from "@angular/material/dialog";
import {VassalSearchResultModel} from "../../../../dtos/vassal-search-result.model";
import {computeAttackDamage} from "../../helpers/mission-math";

@Component({
  templateUrl: './fight-giant.dialog.html',
  styleUrls: ['./fight-giant.dialog.scss']
})
export class FightGiantDialog {

  selectedVassals: VassalSearchResultModel[] = [];

  embarking = false;

  tags: { title: string, color: string }[];
  element: string;

  constructor(
    @Inject(MAT_DIALOG_DATA) data: any,
    private dialogRef: MatDialogRef<FightGiantDialog>,
    private api: ApiService
  ) {
    this.tags = data.tags;
    this.element = data.element;
  }

  attackDamage = (v: VassalSearchResultModel) => computeAttackDamage([ v ], this.element).toString();

  doGoOnMission()
  {
    if(this.embarking)
      return;

    this.embarking = true;
    this.dialogRef.disableClose = true;

    const data = {
      vassals: this.selectedVassals.map(v => v.id)
    }

    this.api.post('alliances/attackGiant', data).subscribe({
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

  static open(matDialog: MatDialog, tags: { title: string, color: string }[], element: string): MatDialogRef<FightGiantDialog>
  {
    return matDialog.open(FightGiantDialog, {
      width: '100%',
      maxWidth: '640px',
      panelClass: 'big',
      data: {
        tags: tags,
        element: element,
      }
    });
  }
}
