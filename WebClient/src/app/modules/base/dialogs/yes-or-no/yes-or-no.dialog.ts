import { Component, Inject } from '@angular/core';
import { MAT_DIALOG_DATA, MatDialog, MatDialogRef } from "@angular/material/dialog";
import { Observable } from "rxjs";

@Component({
  templateUrl: './yes-or-no.dialog.html',
  styleUrls: ['./yes-or-no.dialog.scss']
})
export class YesOrNoDialog {

  working = false;
  callback: () => Observable<boolean>;
  title: string;
  body: string;

  constructor(
    @Inject(MAT_DIALOG_DATA) data: any,
    private dialogRef: MatDialogRef<YesOrNoDialog>
  ) {
    this.callback = data.callback;
    this.title = data.title;
    this.body = data.body;
  }

  doConfirm()
  {
    if(this.working)
      return;

    this.working = true;
    this.dialogRef.disableClose = true;

    this.callback().subscribe({
      next: (success: boolean) => {
        if(success)
        {
          this.dialogRef.close(true);
        }
        else
        {
          this.working = false;
          this.dialogRef.disableClose = false;
        }
      }
    })
  }

  doCancel()
  {
    this.dialogRef.close();
  }

  static open(matDialog: MatDialog, callback: () => Observable<boolean>, title: string = 'Confirm', body: string = 'Are you sure?'): MatDialogRef<YesOrNoDialog>
  {
    return matDialog.open(YesOrNoDialog, {
      data: {
        callback: callback,
        title: title,
        body: body
      }
    });
  }

}
