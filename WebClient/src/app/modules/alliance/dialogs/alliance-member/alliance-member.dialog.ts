import {Component, Inject} from '@angular/core';
import {MAT_DIALOG_DATA, MatDialog, MatDialogRef} from "@angular/material/dialog";
import { ApiService } from "../../../base/services/api.service";
import { catchError, map, of } from "rxjs";
import {ChangeAppearanceDialog} from "../../../game-common/dialogs/change-appearance/change-appearance.dialog";
import {ChangeRibbonDialog} from "../../../game-common/dialogs/change-ribbon/change-ribbon.dialog";
import {HSL} from "../../../../dtos/hsl.model";
import {ChangeMemberTitleDialog} from "../change-member-title/change-member-title.dialog";
import {YesOrNoDialog} from "../../../base/dialogs/yes-or-no/yes-or-no.dialog";

@Component({
  templateUrl: './alliance-member.dialog.html',
  styleUrls: ['./alliance-member.dialog.scss']
})
export class AllianceMemberDialog {

  rights: string[];
  myId: string;
  memberId: string;
  memberName: string;
  memberTitle: string|null;
  memberAvatar: string;
  memberColor: HSL;

  constructor(
    @Inject(MAT_DIALOG_DATA) data: any, private dialogRef: MatDialogRef<AllianceMemberDialog>,
    private api: ApiService, private matDialog: MatDialog
  ) {
    this.rights = data.rights;
    this.myId = data.myId;
    this.memberId = data.memberId;
    this.memberName = data.memberName;
    this.memberTitle = data.memberTitle;
    this.memberAvatar = data.memberAvatar;
    this.memberColor = data.memberColor;
  }

  doChangeAvatar()
  {
    ChangeAppearanceDialog.open(this.matDialog, this.memberAvatar)
      .afterClosed()
      .subscribe(data => {
        if(!data)
          return;

        this.dialogRef.close({ changedAppearance: data });
      })
    ;
  }

  doChangeRibbon()
  {
    ChangeRibbonDialog.open(this.matDialog, this.memberName, this.memberColor)
      .afterClosed()
      .subscribe(data => {
        if(!data)
          return;

        this.dialogRef.close({ changedAppearance: data });
      })
    ;
  }

  doLeave()
  {
    YesOrNoDialog.open(this.matDialog, () => this.doReallyLeave(), 'Leave Alliance').afterClosed().subscribe({
      next: left => {
        if(left)
          this.dialogRef.close({ left: true });
      }
    });
  }

  doReallyLeave()
  {
    return this.api.post('alliances/leave')
      .pipe(
        map(() => true),
        catchError(() => of(false))
      )
    ;
  }

  doKick()
  {
    YesOrNoDialog.open(this.matDialog, () => this.doReallyKick(), 'Kick ' + this.memberName).afterClosed().subscribe({
      next: kicked => {
        if(kicked)
          this.dialogRef.close({ kicked: true });
      }
    });
  }

  doReallyKick()
  {
    return this.api.post('alliances/members/' + this.memberId + '/kick')
      .pipe(
        map(() => true),
        catchError(() => of(false))
      )
    ;
  }

  doChangeTitle()
  {
    ChangeMemberTitleDialog.open(this.matDialog, this.memberId, this.memberTitle).afterClosed().subscribe({
      next: newTitle => {
        if(newTitle)
          this.dialogRef.close({ changedTitle: newTitle });
      }
    });
  }

  doCancel()
  {
    this.dialogRef.close();
  }

  public static open(matDialog: MatDialog, rights: string[], myId: string, memberId: string, memberName: string, memberTitle: string|null, memberAvatar: string, memberColor: HSL): MatDialogRef<AllianceMemberDialog>
  {
    return matDialog.open(AllianceMemberDialog, {
      data: {
        rights: rights,
        myId: myId,
        memberId: memberId,
        memberName: memberName,
        memberTitle: memberTitle,
        memberAvatar: memberAvatar,
        memberColor: memberColor
      }
    })
  }
}
