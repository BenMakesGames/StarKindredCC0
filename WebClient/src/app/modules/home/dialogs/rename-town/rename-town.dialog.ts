import {Component, Inject} from '@angular/core';
import {MAT_DIALOG_DATA, MatDialog, MatDialogRef} from "@angular/material/dialog";
import {ApiService} from "../../../base/services/api.service";

@Component({
  templateUrl: './rename-town.dialog.html',
  styleUrls: ['./rename-town.dialog.scss']
})
export class RenameTownDialog {

  renaming = false;
  name!: string;

  constructor(
    @Inject(MAT_DIALOG_DATA) data: any,
    private dialogRef: MatDialogRef<RenameTownDialog>,
    private api: ApiService
  ) {
    this.name = data.oldName;
  }

  doRename()
  {
    if(this.renaming)
      return;

    this.renaming = true;
    this.dialogRef.disableClose = true;

    this.api.post('towns/my/rename', { name: this.name }).subscribe({
      next: _ => {
        this.dialogRef.close(this.name.trim());
      },
      error: () => {
        this.renaming = false;
        this.dialogRef.disableClose = false;
      }
    })
  }

  doClose()
  {
    this.dialogRef.close();
  }

  static open(matDialog: MatDialog, oldName: string): MatDialogRef<RenameTownDialog>
  {
    return matDialog.open(RenameTownDialog, {
      data: {
        oldName: oldName
      }
    });
  }

}
