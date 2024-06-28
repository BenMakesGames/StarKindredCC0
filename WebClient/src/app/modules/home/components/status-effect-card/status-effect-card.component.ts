import {Component, Input, OnChanges, SimpleChanges} from '@angular/core';

@Component({
  selector: 'app-status-effect-card',
  templateUrl: './status-effect-card.component.html',
  styleUrls: ['./status-effect-card.component.scss']
})
export class StatusEffectCardComponent implements OnChanges {

  static readonly GoodTypes: string[] = [
    'Focused',
    'GoldenTouch',
    'ArtisticVision',
    'Power'
  ];

  public static readonly SpecialDurationTypes: string[] = [
    'Focused',
    'GoldenTouch',
    'ArtisticVision',
  ];

  @Input() type!: string;
  @Input() duration!: number;

  ribbonColor = '#900';
  showDuration = true;

  constructor() { }

  ngOnChanges(changes: SimpleChanges) {
    this.ribbonColor = StatusEffectCardComponent.GoodTypes.indexOf(this.type) >= 0 ? '#0b0' : '#f00';
    this.showDuration = StatusEffectCardComponent.SpecialDurationTypes.indexOf(this.type) < 0;
  }
}
