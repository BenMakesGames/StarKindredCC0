import { Component } from '@angular/core';
import {MatDialog, MatDialogRef} from "@angular/material/dialog";
import {ApiService} from "../../../base/services/api.service";

@Component({
  templateUrl: './create-rank.dialog.html',
  styleUrls: ['./create-rank.dialog.scss']
})
export class CreateRankDialog {

  submitting = false;

  title = '';
  rank = 0;
  canRecruit = false;
  canKick = false;
  canTrackGiants = false;

  constructor(private dialogRef: MatDialogRef<CreateRankDialog>, private api: ApiService) { }

  doCreate()
  {
    if(this.submitting) return;

    this.submitting = true;
    this.dialogRef.disableClose = true;

    const data = {
      title: this.title.trim(),
      rank: this.rank,
      canRecruit: this.canRecruit,
      canKick: this.canKick,
      canTrackGiants: this.canTrackGiants
    };

    this.api.post<any>('alliances/titles', data).subscribe({
      next: r => {
        this.dialogRef.close(r.data);
      },
      error: () => {
        this.submitting = false;
        this.dialogRef.disableClose = false;
      }
    })
  }

  doClose()
  {
    this.dialogRef.close();
  }

  public static open(matDialog: MatDialog): MatDialogRef<CreateRankDialog>
  {
    return matDialog.open(CreateRankDialog);
  }
}
