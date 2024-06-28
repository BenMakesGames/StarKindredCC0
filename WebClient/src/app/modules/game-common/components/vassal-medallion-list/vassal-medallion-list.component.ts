import {Component, Input} from '@angular/core';

@Component({
  selector: 'app-vassal-medallion-list',
  templateUrl: './vassal-medallion-list.component.html',
  styleUrls: ['./vassal-medallion-list.component.scss']
})
export class VassalMedallionListComponent {

  @Input() vassals!: VassalInfo [];

}

interface VassalInfo
{
  id: string;
  level: number;
  species: string;
  portrait: string;
  element: string;
}
