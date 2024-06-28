import {Component, Input} from '@angular/core';

@Component({
  selector: 'app-vassal-medallion',
  templateUrl: './vassal-medallion.component.html',
  styleUrls: ['./vassal-medallion.component.scss']
})
export class VassalMedallionComponent {

  @Input() portrait!: string;
  @Input() species!: string;
  @Input() level!: number;
  @Input() element!: string;

}
