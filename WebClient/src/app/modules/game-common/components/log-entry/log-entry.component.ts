import {Component, Input, OnInit} from '@angular/core';

@Component({
  selector: 'app-log-entry',
  templateUrl: './log-entry.component.html',
  styleUrls: ['./log-entry.component.scss']
})
export class LogEntryComponent implements OnInit {

  @Input() createdOn!: string;
  @Input() message!: string;
  @Input() tags: string[]|null = null;
  @Input() icon: string|null = null;

  constructor() { }

  ngOnInit(): void {
  }

}
