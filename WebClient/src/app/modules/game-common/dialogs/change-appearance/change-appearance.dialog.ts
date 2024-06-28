import {Component, Inject, OnInit} from '@angular/core';
import {MAT_DIALOG_DATA, MatDialog, MatDialogRef} from "@angular/material/dialog";
import {ApiService} from "../../../base/services/api.service";

@Component({
  templateUrl: './change-appearance.dialog.html',
  styleUrls: ['./change-appearance.dialog.scss']
})
export class ChangeAppearanceDialog implements OnInit {

  avatars: string[]|null = null;
  saving = false;

  avatar: string;

  constructor(
    @Inject(MAT_DIALOG_DATA) data: any,
    private dialogRef: MatDialogRef<ChangeAppearanceDialog>,
    private api: ApiService
  ) {
    this.avatar = data.avatar;
  }

  ngOnInit() {
    this.api.get<{ avatars: string[] }>('accounts/availableAvatars').subscribe({
      next: r => {
        this.avatars = r.data.avatars;
      }
    });
  }

  doSaveChanges()
  {
    if(this.saving)
      return;

    this.saving = true;
    this.dialogRef.disableClose = true;

    const data = { avatar: this.avatar };

    this.api.post('accounts/changeAppearance', data).subscribe({
      next: () => {
        this.dialogRef.close(data);
      },
      error: () => {
        this.saving = false;
        this.dialogRef.disableClose = false;
      }
    });
  }


  doClose()
  {
    this.dialogRef.close();
  }

  static open(matDialog: MatDialog, avatar: string): MatDialogRef<ChangeAppearanceDialog>
  {
    return matDialog.open(ChangeAppearanceDialog, {
      data: {
        avatar: avatar
      },
      maxWidth: '640px'
    })
  }

}
