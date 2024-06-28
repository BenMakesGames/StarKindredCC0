import {Component, Inject} from '@angular/core';
import {MAT_DIALOG_DATA, MatDialog, MatDialogRef} from "@angular/material/dialog";

@Component({
  templateUrl: './help.dialog.html',
  styleUrls: ['./help.dialog.scss']
})
export class HelpDialog {

  key: string;
  title: string|null;

  constructor(
    @Inject(MAT_DIALOG_DATA) data: any,
    private dialogRef: MatDialogRef<HelpDialog>
  ) {
    this.key = data.key.toLowerCase()
      .replace(/[^a-z0-9\-]/g, '')
      .replace(/^-/, '')
      .replace(/-$/, '')
      .replace(/-+/g, '/')
    ;
    this.title = data.title;
  }

  doClose()
  {
    this.dialogRef.close();
  }

  static open(matDialog: MatDialog, key: string, title: string|null = null): MatDialogRef<HelpDialog>
  {
    return matDialog.open(HelpDialog, {
      data: {
        key: key,
        title: title
      },
      maxWidth: '720px'
    })
  }

}
