import {Component, Inject, OnInit} from '@angular/core';
import {MAT_DIALOG_DATA, MatDialog, MatDialogRef} from "@angular/material/dialog";
import {VassalSearchResultModel} from "../../../../dtos/vassal-search-result.model";
import {ApiService} from "../../../base/services/api.service";

@Component({
  templateUrl: './assign-vassal-to-position.dialog.html',
  styleUrls: ['./assign-vassal-to-position.dialog.scss']
})
export class AssignVassalToPositionDialog implements OnInit {

  assigning = false;

  position: string;
  tags: { title: string, color: string }[];

  constructor(
    @Inject(MAT_DIALOG_DATA) data: any,
    private dialogRef: MatDialogRef<AssignVassalToPositionDialog>,
    private api: ApiService
  ) {
    this.position = data.position;
    this.tags = data.tags;
  }

  ngOnInit(): void {
  }

  cancel()
  {
    this.dialogRef.close();
  }

  doSelectVassal(vassal: VassalSearchResultModel)
  {
    this.assigning = true;
    this.dialogRef.disableClose = true;

    this.api.post('leaders', { vassalId: vassal.id, position: this.position }).subscribe({
      next: () => {
        this.dialogRef.close(vassal);
      },
      error: () => {
        this.assigning = false;
        this.dialogRef.disableClose = false;
      }
    });
  }

  projectCost = (vassal: VassalSearchResultModel) =>
  {
    return <string>({
      Cavalier: 'Iron',
      Competitor: 'Gold',
      Defender: 'Stone',
      Explorer: 'Wood',
      Evangelist: 'Quintessence',
      Loner: 'Stone',
      Monger: 'Gold',
      Perfectionist: 'Marble',
      ThrillSeeker: 'Wine',
      Visionary: 'Quintessence'
    }[vassal.nature]);
  };

  static open(matDialog: MatDialog, tags: { title: string, color: string }[], position: string): MatDialogRef<AssignVassalToPositionDialog>
  {
    return matDialog.open(AssignVassalToPositionDialog, {
      data: {
        tags: tags,
        position: position
      }
    });
  }
}
