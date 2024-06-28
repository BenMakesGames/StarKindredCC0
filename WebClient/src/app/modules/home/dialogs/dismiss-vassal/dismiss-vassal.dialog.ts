import {Component, Inject} from '@angular/core';
import {MAT_DIALOG_DATA, MatDialog, MatDialogRef} from "@angular/material/dialog";
import {ApiService} from "../../../base/services/api.service";

@Component({
  templateUrl: './dismiss-vassal.dialog.html',
  styleUrls: ['./dismiss-vassal.dialog.scss']
})
export class DismissVassalDialog {

  dismissing = false;
  confirm = false;

  vassalId: string;
  vassalName: string;
  vassalSpecies: string;
  vassalPortrait: string;
  vassalLevel: number;
  vassalNature: string;

  constructor(
    @Inject(MAT_DIALOG_DATA) data: any,
    private dialogRef: MatDialogRef<DismissVassalDialog>,
    private api: ApiService
  ) {
    this.vassalId = data.vassalId;
    this.vassalName = data.vassalName;
    this.vassalSpecies = data.vassalSpecies;
    this.vassalPortrait = data.vassalPortrait;
    this.vassalLevel = data.vassalLevel;
    this.vassalNature = data.vassalNature;
  }

  doDismiss()
  {
    if(this.dismissing)
      return;

    this.dismissing = true;
    this.dialogRef.disableClose = true;

    this.api.post('vassals/' + this.vassalId + '/dismiss').subscribe({
      next: _ => {
        this.dialogRef.close(true);
      },
      error: () => {
        this.dismissing = false;
        this.dialogRef.disableClose = false;
      }
    })
  }

  doClose()
  {
    this.dialogRef.close();
  }

  static open(matDialog: MatDialog, vassalId: string, name: string, species: string, portrait: string, level: number, nature: string): MatDialogRef<DismissVassalDialog>
  {
    return matDialog.open(DismissVassalDialog, {
      data: {
        vassalId: vassalId,
        vassalName: name,
        vassalSpecies: species,
        vassalPortrait: portrait,
        vassalLevel: level,
        vassalNature: nature
      },
      maxWidth: '540px'
    })
  }

}
