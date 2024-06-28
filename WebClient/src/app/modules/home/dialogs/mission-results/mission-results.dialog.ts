import {Component, Inject} from '@angular/core';
import {MAT_DIALOG_DATA, MatDialog, MatDialogRef} from "@angular/material/dialog";
import {CompleteMissionResponseDto} from "../../../../dtos/complete-mission-response.dto";

@Component({
  templateUrl: './mission-results.dialog.html',
  styleUrls: ['./mission-results.dialog.scss']
})
export class MissionResultsDialog {

  report: CompleteMissionResponseDto;
  title: string;

  constructor(
    @Inject(MAT_DIALOG_DATA) data: any,
    private dialogRef: MatDialogRef<MissionResultsDialog>
  ) {
    this.report = data.report;

    if(this.report.outcome == 'Bad')
      this.title = 'Oops!';
    else if(this.report.outcome == 'Good')
      this.title = 'Success';
    else //if(this.report.outcome == 'Great')
      this.title = 'Great Success!';
  }

  doOk()
  {
    this.dialogRef.close();
  }

  static open(matDialog: MatDialog, report: CompleteMissionResponseDto): MatDialogRef<MissionResultsDialog>
  {
    return matDialog.open(MissionResultsDialog, {
      data: {
        report: report
      }
    });
  }
}
