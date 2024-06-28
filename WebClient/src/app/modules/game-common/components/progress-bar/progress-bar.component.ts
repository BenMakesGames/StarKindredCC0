import { Component, Input, OnChanges, SimpleChanges } from '@angular/core';

@Component({
  selector: 'app-progress-bar',
  templateUrl: './progress-bar.component.html',
  styleUrls: ['./progress-bar.component.scss']
})
export class ProgressBarComponent implements OnChanges {

  @Input() progress = 0;
  @Input() maxProgress = 100;
  @Input() pulsing = true;

  progressPercent = 0;

  ngOnChanges(changes: SimpleChanges): void
  {
    this.progressPercent = Math.min(100, this.progress * 100 / this.maxProgress);
  }

}
