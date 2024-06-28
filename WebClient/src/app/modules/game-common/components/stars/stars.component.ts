import {Component, Input, OnChanges, SimpleChanges} from '@angular/core';

@Component({
  selector: 'app-stars',
  templateUrl: './stars.component.html',
  styleUrls: ['./stars.component.scss']
})
export class StarsComponent implements OnChanges {

  @Input() title!: string;
  @Input() stars!: number;
  @Input() icon!: string;
  @Input() emptyIcon: string|null = null;
  @Input() maxStars: number|null = null;

  starArray: string[] = [];

  ngOnChanges(changes: SimpleChanges) {
    this.starArray = [];
    for(let i = 0; i < this.stars; i++)
      this.starArray.push(this.icon);

    if(this.maxStars && this.emptyIcon)
    {
      for(let i = this.stars; i < this.maxStars; i++)
        this.starArray.push(this.emptyIcon);
    }
  }

}
