import {Component, Input, OnChanges, SimpleChanges} from '@angular/core';

@Component({
  selector: 'app-vassal-portrait',
  templateUrl: './vassal-portrait.component.html',
  styleUrls: ['./vassal-portrait.component.scss']
})
export class VassalPortraitComponent implements OnChanges {
  @Input() vassal!: Vassal;

  isNew = false;

  ngOnChanges(changes: SimpleChanges) {
    this.isNew = Date.now() - Date.parse(this.vassal.recruitDate) < 12 * 60 * 60 * 1000;
  }
}

export interface Vassal
{
  favorite: boolean;
  element: string;
  name: string;
  portrait: string;
  species: string;
  level: number;
  recruitDate: string;
  willpower: number;
  statusEffects: string[];
  onMission: boolean;
  leader: boolean;
  weapon: { image: string, level: number } | null;
}
