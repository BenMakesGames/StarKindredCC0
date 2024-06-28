import {Component, Input, OnInit} from '@angular/core';

@Component({
  selector: 'app-research-time',
  templateUrl: './research-time.component.html',
  styleUrls: ['./research-time.component.scss']
})
export class ResearchTimeComponent implements OnInit {

  @Input() time!: number;

  constructor() { }

  ngOnInit(): void {
  }

}
