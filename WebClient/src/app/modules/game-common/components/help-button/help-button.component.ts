import {Component, Input} from '@angular/core';
import {HelpDialog} from "../../dialogs/help/help.dialog";
import {MatDialog} from "@angular/material/dialog";

@Component({
  selector: 'app-help-button',
  template: '<button type="button" (click)="doShowHelp(topic)">?</button>',
  styleUrls: ['./help-button.component.scss']
})
export class HelpButtonComponent {

  @Input() topic!: string;

  constructor(private matDialog: MatDialog) { }

  doShowHelp(key: string)
  {
    HelpDialog.open(this.matDialog, key);
  }

}
