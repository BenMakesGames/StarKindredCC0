import {Component, Input, OnInit} from '@angular/core';

@Component({
  selector: 'app-generic-card',
  templateUrl: './generic-card.component.html',
  styleUrls: ['./generic-card.component.scss']
})
export class GenericCardComponent implements OnInit {

  @Input() ribbonColor = '#69c';
  @Input() ribbonText!: string;
  @Input() helpTopic: string|null = null;
  @Input() image!: string;
  @Input() imageWidth = 75;

  constructor() { }

  ngOnInit(): void {
  }

}
