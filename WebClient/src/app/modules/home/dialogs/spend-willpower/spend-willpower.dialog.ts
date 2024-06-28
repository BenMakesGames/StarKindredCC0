import {Component, Inject, OnInit} from '@angular/core';
import {MAT_DIALOG_DATA, MatDialog, MatDialogRef} from "@angular/material/dialog";
import {ApiService} from "../../../base/services/api.service";

@Component({
  selector: 'app-spend-willpower',
  templateUrl: './spend-willpower.dialog.html',
  styleUrls: ['./spend-willpower.dialog.scss']
})
export class SpendWillpowerDialog implements OnInit {

  vassalId: string;
  vassalLevel: number;
  willpower: number;
  maxWillpower: number;

  options: WillpowerOption[]|null = null;

  selecting = false;
  selected: string|null = null;

  constructor(
    private api: ApiService, private dialogRef: MatDialogRef<SpendWillpowerDialog>,
    @Inject(MAT_DIALOG_DATA) data: any
  )
  {
    this.vassalId = data.vassalId;
    this.vassalLevel = data.vassalLevel;
    this.willpower = data.willpower;
    this.maxWillpower = data.maxWillpower;
  }

  ngOnInit(): void {
    this.api.get<{ options: WillpowerOption[] }>('vassals/' + this.vassalId + '/willpowerOptions').subscribe({
      next: r => {
        this.options = r.data.options;
      }
    })
  }

  doSubmit()
  {
    if(this.selecting)
      return;

    this.selecting = true;
    this.dialogRef.disableClose = true;

    this.api.post('vassals/' + this.vassalId + '/spendWillpower', { selection: this.selected }).subscribe({
      next: _ => {
        this.dialogRef.close(true);
      },
      error: () => {
        this.selecting = false;
        this.dialogRef.disableClose = false;
      }
    })
  }

  doCancel()
  {
    this.dialogRef.close();
  }

  static open(matDialog: MatDialog, vassalId: string, vassalLevel: number, willpower: number, maxWillpower: number): MatDialogRef<SpendWillpowerDialog>
  {
    return matDialog.open(SpendWillpowerDialog, {
      data: {
        vassalId: vassalId,
        vassalLevel: vassalLevel,
        willpower: willpower,
        maxWillpower: maxWillpower
      }
    })
  }
}

interface WillpowerOption
{
  type: string;
  cost: number;
  canUse: boolean;
}
