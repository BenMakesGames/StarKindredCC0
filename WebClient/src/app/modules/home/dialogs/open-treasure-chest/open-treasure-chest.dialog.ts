import {Component, Inject} from '@angular/core';
import {MAT_DIALOG_DATA, MatDialog, MatDialogRef} from "@angular/material/dialog";
import {ApiService} from "../../../base/services/api.service";

@Component({
  templateUrl: './open-treasure-chest.dialog.html',
  styleUrls: ['./open-treasure-chest.dialog.scss']
})
export class OpenTreasureChestDialog {

  opening = false;
  endPoint: string;
  choice: string|undefined;
  choices: TreasureChoice[];
  useLabel!: string;
  maxAllowedQuantity: number;
  quantity = 1;

  constructor(
    @Inject(MAT_DIALOG_DATA) data: any,
    private dialogRef: MatDialogRef<OpenTreasureChestDialog>,
    private api: ApiService
  ) {
    this.endPoint = data.endPoint;
    this.choices = data.choices;
    this.useLabel = data.useLabel;
    this.maxAllowedQuantity = data.maxAllowedQuantity;
  }

  doOpen()
  {
    if(this.opening)
      return;

    this.opening = true;
    this.dialogRef.disableClose = true;

    const data = this.choice == null ? null : { choice: this.choice, quantity: this.quantity };

    this.api.post('treasures/use/' + this.endPoint, data).subscribe({
      next: _ => {
        this.dialogRef.close(true);
      },
      error: () => {
        this.opening = false;
        this.dialogRef.disableClose = false;
      }
    })
  }

  doClose()
  {
    this.dialogRef.close();
  }

  static open(matDialog: MatDialog, endPoint: string, choices: TreasureChoice[], maxAllowedQuantity: number, useLabel: string): MatDialogRef<OpenTreasureChestDialog>
  {
    return matDialog.open(OpenTreasureChestDialog, {
      data: {
        endPoint: endPoint,
        choices: choices,
        maxAllowedQuantity: maxAllowedQuantity,
        useLabel: useLabel
      },
    })
  }

}

export interface TreasureChoice
{
  label: string;
  value: string|null;
  image: string;
  techRequirement?: string;
}
