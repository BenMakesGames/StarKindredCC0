import {Component, Input, OnChanges, OnInit} from '@angular/core';

@Component({
  selector: 'app-weapon-card',
  templateUrl: './weapon-card.component.html',
  styleUrls: ['./weapon-card.component.scss']
})
export class WeaponCardComponent implements OnChanges {

  @Input() name!: string;
  @Input() image!: string;
  @Input() level: number|null = null;
  @Input() repairValue: number|null = null;
  @Input() vassal: { level: number, species: string, portrait: string, element: string }|null = null;

  adjective: string|null = null;

  constructor() { }

  static legitAdjectives = [
    'bird-s', 'black', 'blue', 'butterfly', 'cherry', 'dark', 'flying',
    'jeweled', 'light', 'moon', 'shining', 'silver', 'singing',
    'stinging', 'sun'
  ];

  ngOnChanges(): void {
    const adjective = this.name.split(' ')[0].toLowerCase().replace(/[^a-z]+/g, '-');

    if(WeaponCardComponent.legitAdjectives.includes(adjective)) {
      this.adjective = adjective;
    }
  }
}
