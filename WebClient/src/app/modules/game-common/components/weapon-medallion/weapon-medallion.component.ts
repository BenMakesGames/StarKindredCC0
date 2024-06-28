import {Component, Input} from '@angular/core';

@Component({
  selector: 'app-weapon-medallion',
  templateUrl: './weapon-medallion.component.html',
  styleUrls: ['./weapon-medallion.component.scss']
})
export class WeaponMedallionComponent {

  @Input() image!: string;
  @Input() level!: number;

}
