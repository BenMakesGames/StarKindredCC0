import {Component, Inject, OnInit} from '@angular/core';
import {MAT_DIALOG_DATA, MatDialog, MatDialogRef} from "@angular/material/dialog";
import {ApiService} from "../../../base/services/api.service";
import {HSL} from "../../../../dtos/hsl.model";

@Component({
  templateUrl: './change-ribbon.dialog.html',
  styleUrls: ['./change-ribbon.dialog.scss']
})
export class ChangeRibbonDialog implements OnInit {

  saving = false;

  color: HSL;
  name: string;

  constructor(
    @Inject(MAT_DIALOG_DATA) data: any,
    private dialogRef: MatDialogRef<ChangeRibbonDialog>,
    private api: ApiService
  ) {
    this.color = data.color;
    this.name = data.name;
  }

  ngOnInit() {
  }

  doSaveChanges()
  {
    if(this.saving)
      return;

    this.saving = true;
    this.dialogRef.disableClose = true;

    const data = { color: this.color };

    this.api.post('accounts/changeRibbon', data).subscribe({
      next: () => {
        this.dialogRef.close(data);
      },
      error: () => {
        this.saving = false;
        this.dialogRef.disableClose = false;
      }
    });
  }

  doUpdateHue(event: Event)
  {
    this.color = { ...this.color, hue: (<any>event.target).value };
  }

  doUpdateSaturation(event: Event)
  {
    this.color = { ...this.color, saturation: (<any>event.target).value };
  }

  doUpdateLuminosity(event: Event)
  {
    this.color = { ...this.color, luminosity: (<any>event.target).value };
  }

  doClose()
  {
    this.dialogRef.close();
  }

  static open(matDialog: MatDialog, name: string, color: HSL): MatDialogRef<ChangeRibbonDialog>
  {
    return matDialog.open(ChangeRibbonDialog, {
      data: {
        name: name,
        color: color
      },
      maxWidth: '640px'
    });
  }

}
