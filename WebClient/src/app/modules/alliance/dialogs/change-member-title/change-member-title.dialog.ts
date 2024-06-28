import {Component, Inject, OnInit} from '@angular/core';
import {MAT_DIALOG_DATA, MatDialog, MatDialogRef} from "@angular/material/dialog";
import {ApiService} from "../../../base/services/api.service";

@Component({
  templateUrl: './change-member-title.dialog.html',
  styleUrls: ['./change-member-title.dialog.scss']
})
export class ChangeMemberTitleDialog implements OnInit {

  submitting = false;

  memberId: string;
  titleName: string|null;

  titleId: string|null = null;
  titles: TitleDto[]|null = null;

  constructor(
    private dialogRef: MatDialogRef<ChangeMemberTitleDialog>, private api: ApiService,
    @Inject(MAT_DIALOG_DATA) data: any
  ) {
    this.memberId = data.memberId;
    this.titleName = data.currentTitle;
  }

  ngOnInit() {
    this.api.get<{ titles: TitleDto[] }>('alliances/titles').subscribe({
      next: r => {
        this.titles = r.data.titles;

        this.titles.sort((a, b) => b.rank - a.rank);

        this.titleId = this.titles.find(t => t.title == this.titleName)?.id ?? null;
      }
    });
  }

  doChangeTitle()
  {
    if(this.submitting) return;

    this.submitting = true;
    this.dialogRef.disableClose = true;

    const data = {
      titleId: this.titleId
    };

    this.api.post('alliances/members/' + this.memberId + '/changeTitle', data).subscribe({
      next: _ => {
        const titleName = this.titles!.find(t => t.id == this.titleId)?.title ?? "No Title";

        this.dialogRef.close(titleName);
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

  public static open(matDialog: MatDialog, memberId: string, currentTitle: string|null): MatDialogRef<ChangeMemberTitleDialog>
  {
    return matDialog.open(ChangeMemberTitleDialog, {
      data: {
        memberId: memberId,
        currentTitle: currentTitle
      }
    });
  }
}

interface TitleDto
{
  id: string;
  title: string;
  rank: number;
}
