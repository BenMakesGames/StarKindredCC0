import {Component, Inject, OnInit} from '@angular/core';
import {MAT_DIALOG_DATA, MatDialog, MatDialogRef} from "@angular/material/dialog";
import { ApiService } from "../../../base/services/api.service";

@Component({
  templateUrl: './recruit.dialog.html',
  styleUrls: ['./recruit.dialog.scss']
})
export class RecruitDialog implements OnInit {

  waiting = false;

  useInviteCode = false;

  openInvitation = false;
  minLevel = 0;
  maxLevel = 330;

  inviteStatus: InviteStatusDto|null = null;

  constructor(
    @Inject(MAT_DIALOG_DATA) data: any, private dialogRef: MatDialogRef<RecruitDialog>,
    private api: ApiService
  ) {
  }

  ngOnInit() {
    this.api.get<InviteStatusDto>('alliances/inviteStatus').subscribe({
      next: r => {
        this.inviteStatus = r.data;

        this.openInvitation = this.inviteStatus.usingOpenInvitation;
        this.minLevel = this.inviteStatus.openInvitationMinLevel;
        this.maxLevel = this.inviteStatus.openInvitationMaxLevel;

        this.useInviteCode = this.inviteStatus.usingInviteCode;
      }
    })
  }

  doUpdateOpenInvitation()
  {
    if(this.waiting)
      return;

    this.waiting = true;
    this.dialogRef.disableClose = true;

    if(this.openInvitation)
    {
      const data = { minLevel: this.minLevel, maxLevel: this.maxLevel };

      this.api.post('alliances/enableOpenInvitation', data).subscribe({
        next: _ => {
          this.doneWaiting();
        },
        error: () => {
          this.doneWaiting();
        }
      });
    }
    else
    {
      this.api.post('alliances/disableOpenInvitation').subscribe({
        next: _ => {
          this.doneWaiting();
        },
        error: () => {
          this.doneWaiting()
        }
      });
    }
  }

  private doneWaiting()
  {
    this.waiting = false;
    this.dialogRef.disableClose = false;
  }

  doUpdateInviteCode()
  {
    if(this.waiting)
      return;

    this.waiting = true;
    this.dialogRef.disableClose = true;

    if(this.useInviteCode)
    {
      this.api.post('alliances/enableInviteCode').subscribe({
        next: _ => {
          this.doneWaiting();
        },
        error: () => {
          this.doneWaiting();
        }
      });
    }
    else
    {
      this.api.post('alliances/disableInviteCode').subscribe({
        next: _ => {
          this.doneWaiting();
        },
        error: () => {
          this.doneWaiting();
        }
      });
    }
  }

  doCancel()
  {
    this.dialogRef.close();
  }

  static open(matDialog: MatDialog): MatDialogRef<RecruitDialog>
  {
    return matDialog.open(RecruitDialog);
  }
}

interface InviteStatusDto
{
  usingInviteCode: boolean;
  inviteCode: string;
  usingOpenInvitation: boolean;
  openInvitationMinLevel: number;
  openInvitationMaxLevel: number;
}
