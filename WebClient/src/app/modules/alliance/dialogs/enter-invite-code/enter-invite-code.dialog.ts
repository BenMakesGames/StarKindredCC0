import {Component, Inject} from '@angular/core';
import {MAT_DIALOG_DATA, MatDialog, MatDialogRef} from "@angular/material/dialog";
import {ApiService} from "../../../base/services/api.service";

@Component({
  templateUrl: './enter-invite-code.dialog.html',
  styleUrls: ['./enter-invite-code.dialog.scss']
})
export class EnterInviteCodeDialog {

  submitting = false;
  inviteCode = '';

  constructor(
    private dialogRef: MatDialogRef<EnterInviteCodeDialog>, private api: ApiService,
    @Inject(MAT_DIALOG_DATA) data: any
  ) {
  }

  doRename()
  {
    if(this.submitting) return;

    this.submitting = true;
    this.dialogRef.disableClose = true;

    const data = {
      inviteCode: this.inviteCode,
    };

    this.api.post<{ allianceId: string }>('alliances/join', data).subscribe({
      next: r => {
        this.dialogRef.close(r.data.allianceId);
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

  public static open(matDialog: MatDialog): MatDialogRef<EnterInviteCodeDialog>
  {
    return matDialog.open(EnterInviteCodeDialog);
  }
}
