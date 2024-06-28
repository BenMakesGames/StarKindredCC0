import {Component, Input} from '@angular/core';

@Component({
  selector: 'app-level-badge',
  template: `
    <div
      class="level"
      [style.width.em]="size * 2"
      [style.height.em]="size * 2"
      [style.line-height.em]="size * 2"
      [style.font-size.em]="size * .8"
    >{{ level }}</div>`,
  styleUrls: ['./level-badge.component.scss']
})
export class LevelBadgeComponent {

  @Input() level!: number;
  @Input() size: number = 1;

}
