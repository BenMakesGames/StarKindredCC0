import {Component, Inject} from '@angular/core';
import {MAT_DIALOG_DATA, MatDialog, MatDialogRef} from "@angular/material/dialog";
import {ApiService} from "../../../base/services/api.service";

@Component({
  templateUrl: './add-tag.dialog.html',
  styleUrls: ['./add-tag.dialog.scss']
})
export class AddTagDialog {

  availableColors = [
    'ff9999', 'ee88ee', '99cccc', '99dd99', 'dddd77', 'ffbb88',
    'ffffff', 'bbbbbb',
  ];

  adding = false;
  vassalId: string;

  title: string = '';
  color: string;

  tags: { title: string, color: string }[]|null = null;

  constructor(
    @Inject(MAT_DIALOG_DATA) data: any,
    private dialogRef: MatDialogRef<AddTagDialog>,
    private api: ApiService
  ) {
    this.vassalId = data.vassalId;

    this.color = this.availableColors[Math.floor(Math.random() * this.availableColors.length)];

    this.api.get<{ tags: { title: string, color: string }[] }>(`accounts/tags`).subscribe({
      next: r => {
        this.tags = r.data.tags;
      }
    });
  }

  doSelect(tag: { title: string, color: string })
  {
    this.post(tag.title, tag.color);
  }

  doAdd() {
    this.post(this.title, this.color);
  }

  private post(title: string, color: string)
  {
    if(this.adding)
      return;

    this.adding = true;
    this.dialogRef.disableClose = true;

    this.api.post('vassals/' + this.vassalId + '/tags', { title: title, color: color }).subscribe({
      next: _ => {
        this.dialogRef.close({ title: title, color: color });
      },
      error: () => {
        this.adding = false;
        this.dialogRef.disableClose = false;
      }
    })
  }

  doClose()
  {
    this.dialogRef.close();
  }

  static open(matDialog: MatDialog, vassalId: string): MatDialogRef<AddTagDialog>
  {
    return matDialog.open(AddTagDialog, {
      data: {
        vassalId: vassalId,
      },
      maxWidth: '720px'
    })
  }
}
