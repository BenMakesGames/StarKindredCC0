import {Component, Inject, OnInit} from '@angular/core';
import { ApiService } from "../../../base/services/api.service";
import { MAT_DIALOG_DATA, MatDialog, MatDialogRef } from "@angular/material/dialog";

@Component({
  templateUrl: './show-completed-story.dialog.html',
  styleUrls: ['./show-completed-story.dialog.scss']
})
export class ShowCompletedStoryDialog implements OnInit {

  storyId: string;
  story: { narrative: string }|null = null;

  constructor(
    @Inject(MAT_DIALOG_DATA) data: any,
    private dialogRef: MatDialogRef<ShowCompletedStoryDialog>,
    private api: ApiService
  ) {
    this.storyId = data.storyId;
  }

  ngOnInit() {
    this.api.get<{ narrative: string }>('stories/' + this.storyId + '/narrative').subscribe({
      next: r => {
        this.story = r.data;
      }
    });
  }

  public doOk()
  {
    this.dialogRef.close();
  }

  static open(matDialog: MatDialog, storyId: string): MatDialogRef<ShowCompletedStoryDialog>
  {
    return matDialog.open(ShowCompletedStoryDialog, {
      data: {
        storyId: storyId,
      }
    });
  }
}
