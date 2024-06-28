import {Component, Inject} from '@angular/core';
import {MAT_DIALOG_DATA, MatDialog, MatDialogRef} from "@angular/material/dialog";
import {HelpKeyPipe} from "../../../game-common/pipes/help-key.pipe";
import {ApiService} from "../../../base/services/api.service";
import {StatusEffectCardComponent} from "../../components/status-effect-card/status-effect-card.component";

@Component({
  templateUrl: './status-effect.dialog.html',
  styleUrls: ['./status-effect.dialog.scss']
})
export class StatusEffectDialog {

  removing = false;
  duration: number;
  type: string;
  id: string;
  helpKey: string;
  ichorQuantity: number;
  specialDuration: boolean;

  constructor(
    @Inject(MAT_DIALOG_DATA) data: any,
    private dialogRef: MatDialogRef<StatusEffectDialog>,
    private api: ApiService
  ) {
    this.id = data.id;
    this.type = data.type;
    this.duration = data.duration;
    this.ichorQuantity = data.ichorQuantity;
    this.specialDuration = StatusEffectCardComponent.SpecialDurationTypes.indexOf(this.type) >= 0;

    this.helpKey = new HelpKeyPipe().transform('statuseffect-' + this.type);
  }

  doUseIchor()
  {
    if(this.removing)
      return;

    this.removing = true;
    this.dialogRef.disableClose = true;

    this.api.post('statusEffects/' + this.id + '/remove').subscribe({
      next: _ => {
        this.dialogRef.close(true);
      },
      error: () => {
        this.removing = false;
        this.dialogRef.disableClose = false;
      }
    })
  }

  doClose()
  {
    this.dialogRef.close();
  }

  static open(matDialog: MatDialog, statusEffectId: string, statusEffect: string, duration: number, ichorQuantity: number): MatDialogRef<StatusEffectDialog>
  {
    return matDialog.open(StatusEffectDialog, {
      data: {
        id: statusEffectId,
        type: statusEffect,
        duration: duration,
        ichorQuantity: ichorQuantity
      },
      maxWidth: '360px'
    })
  }
}
