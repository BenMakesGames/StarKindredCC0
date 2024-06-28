import { Component, Inject } from '@angular/core';
import { ApiService } from "../../../base/services/api.service";
import { MAT_DIALOG_DATA, MatDialog, MatDialogRef } from "@angular/material/dialog";
import {VassalSearchResultModel} from "../../../../dtos/vassal-search-result.model";
import {AvailableStoryStepModel} from "../../models/available-story-step.model";

@Component({
  templateUrl: './do-story.dialog.html',
  styleUrls: ['./do-story.dialog.scss']
})
export class DoStoryDialog {

  selectedVassals: VassalSearchResultModel[] = [];

  embarking = false;

  storyStep: AvailableStoryStepModel;
  tags: { title: string, color: string }[];

  constructor(
    @Inject(MAT_DIALOG_DATA) data: any,
    private dialogRef: MatDialogRef<DoStoryDialog>,
    private api: ApiService
  ) {
    this.tags = data.tags;
    this.storyStep = data.storyStep;
  }

  doGoOnMission()
  {
    if(this.embarking)
      return;

    this.embarking = true;
    this.dialogRef.disableClose = true;

    const data = {
      vassalIds: this.selectedVassals.map(v => v.id)
    }

    this.api.post('stories/' + this.storyStep.id + '/start', data).subscribe({
      next: _ => {
        this.dialogRef.close(true);
      },
      error: () => {
        this.embarking = false;
        this.dialogRef.disableClose = false;
      }
    })
  }

  doCancel()
  {
    this.dialogRef.close();
  }

  static open(matDialog: MatDialog, tags: { title: string, color: string }[], storyStep: AvailableStoryStepModel): MatDialogRef<DoStoryDialog>
  {
    return matDialog.open(DoStoryDialog, {
      width: '100%',
      maxWidth: '640px',
      panelClass: 'big',
      data: {
        tags: tags,
        storyStep: storyStep,
      }
    });
  }
}
