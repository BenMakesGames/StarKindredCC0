import {Component, Inject, OnInit} from '@angular/core';
import {MAT_DIALOG_DATA, MatDialog, MatDialogRef} from "@angular/material/dialog";
import {ApiService} from "../../../base/services/api.service";
import {ResourceQuantityDto} from "../../../../dtos/resource-quantity.dto";

@Component({
  templateUrl: './research.dialog.html',
  styleUrls: ['./research.dialog.scss']
})
export class ResearchDialog implements OnInit {

  availableTechnologies: TechInfo[]|null = null;

  selection: string = '';
  submitting = false;

  position: string;

  constructor(
    @Inject(MAT_DIALOG_DATA) data: any,
    private dialogRef: MatDialogRef<ResearchDialog>,
    private api: ApiService
  ) {
    this.position = data.position;
  }

  ngOnInit(): void {
    this.api.get<ResponseDto>('leaders/' + this.position + '/researchOptions').subscribe({
      next: r => {
        this.availableTechnologies = r.data.technologies;
      }
    });
  }

  doCancel()
  {
    this.dialogRef.close();
  }

  doSelect()
  {
    if(this.submitting)
      return;

    this.submitting = true;
    this.dialogRef.disableClose = true;

    const cost = this.availableTechnologies!.find(t => t.value == this.selection)!.cost;

    this.api.post('leaders/research', { research: this.selection }).subscribe({
      next: _ => {
        this.dialogRef.close({ cost: cost });
      },
      error: () => {
        this.submitting = false;
        this.dialogRef.disableClose = false;
      }
    })
  }

  static open(matDialog: MatDialog, position: string): MatDialogRef<ResearchDialog>
  {
    return matDialog.open(ResearchDialog, {
      data: {
        position: position
      }
    });
  }
}

interface ResponseDto
{
  technologies: TechInfo[];
}

interface TechInfo
{
  value: string;
  title: string;
  cost: ResourceQuantityDto[];
  timeInMinutes: number;
}
