import {Component, Input, OnInit} from '@angular/core';

@Component({
  selector: 'app-card-ribbon',
  templateUrl: './card-ribbon.component.html',
  styleUrls: ['./card-ribbon.component.scss']
})
export class CardRibbonComponent implements OnInit {

  @Input() color: string = '#258';

  constructor() { }

  ngOnInit(): void {
  }

}
