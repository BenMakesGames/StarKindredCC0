import { Component, Inject } from '@angular/core';
import { MAT_DIALOG_DATA, MatDialog, MatDialogRef } from "@angular/material/dialog";

@Component({
  templateUrl: './ok.dialog.html',
  styleUrls: ['./ok.dialog.scss']
})
export class OkDialog {

  title: string;
  body: string;

  constructor(
    @Inject(MAT_DIALOG_DATA) data: any,
    private dialogRef: MatDialogRef<OkDialog>
  ) {
    this.title = data.title;
    this.body = data.body;
  }

  doOk()
  {
    this.dialogRef.close();
  }

  static open(matDialog: MatDialog, title: string, body: string): MatDialogRef<OkDialog>
  {
    return matDialog.open(OkDialog, {
      data: {
        title: title,
        body: body
      }
    });
  }
}
