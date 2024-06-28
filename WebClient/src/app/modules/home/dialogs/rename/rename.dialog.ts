import {Component, Inject} from '@angular/core';
import {MAT_DIALOG_DATA, MatDialog, MatDialogRef} from "@angular/material/dialog";
import {ApiService} from "../../../base/services/api.service";

@Component({
  templateUrl: './rename.dialog.html',
  styleUrls: ['./rename.dialog.scss']
})
export class RenameDialog {

  renaming = false;
  vassalId: string;
  name = '';
  placeholder: string;
  renamingScrollQuantity: number;

  constructor(
    @Inject(MAT_DIALOG_DATA) data: any,
    private dialogRef: MatDialogRef<RenameDialog>,
    private api: ApiService
  ) {
    this.vassalId = data.vassalId;
    this.placeholder = data.oldName;
    this.renamingScrollQuantity = data.renamingScrollQuantity;
  }

  doRename()
  {
    if(this.renaming)
      return;

    this.renaming = true;
    this.dialogRef.disableClose = true;

    this.api.post('vassals/' + this.vassalId + '/rename', { name: this.name }).subscribe({
      next: _ => {
        this.dialogRef.close(true);
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

  static open(matDialog: MatDialog, vassalId: string, name: string, renamingScrollQuantity: number): MatDialogRef<RenameDialog>
  {
    return matDialog.open(RenameDialog, {
      data: {
        vassalId: vassalId,
        oldName: name,
        renamingScrollQuantity: renamingScrollQuantity
      },
      maxWidth: '720px'
    })
  }
}
