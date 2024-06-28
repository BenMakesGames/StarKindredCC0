import {Component, Input} from '@angular/core';

@Component({
  selector: 'app-tag',
  template: `<span [style.background-color]="'#' + color">{{ title }}</span>`,
  styleUrls: ['./tag.component.scss']
})
export class TagComponent {

  @Input() title!: string;
  @Input() color!: string;

}
