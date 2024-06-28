import {Component, Inject} from '@angular/core';
import {MAT_DIALOG_DATA, MatDialog, MatDialogRef} from "@angular/material/dialog";
import {ApiService} from "../../../base/services/api.service";

@Component({
  templateUrl: './rename-rank.dialog.html',
  styleUrls: ['./rename-rank.dialog.scss']
})
export class RenameRankDialog {

  submitting = false;

  id: string;
  title: string;

  constructor(
    private dialogRef: MatDialogRef<RenameRankDialog>, private api: ApiService,
    @Inject(MAT_DIALOG_DATA) data: any
  ) {
    this.id = data.id;
    this.title = data.title;
  }

  doRename()
  {
    if(this.submitting) return;

    this.submitting = true;
    this.dialogRef.disableClose = true;

    const data = {
      title: this.title.trim(),
    };

    this.api.post<any>('alliances/titles/' + this.id + '/rename', data).subscribe({
      next: _ => {
        this.dialogRef.close(data.title);
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

  public static open(matDialog: MatDialog, id: string, title: string): MatDialogRef<RenameRankDialog>
  {
    return matDialog.open(RenameRankDialog, {
      data: {
        id: id,
        title: title,
      }
    });
  }
}
