import { Component, Input, OnChanges, SimpleChanges } from '@angular/core';

@Component({
  selector: 'app-time-remainings',
  templateUrl: './time-remainings.component.html',
  styleUrls: ['./time-remainings.component.scss']
})
export class TimeRemainingsComponent implements OnChanges {

  @Input() current!: number;
  @Input() end!: number;
  @Input() suffix: string = 'remaining';

  remaining = '';

  ngOnChanges(changes: SimpleChanges): void
  {
    this.remaining = this.computeTimeRemaining();
  }

  computeTimeRemaining(): string
  {
    if(this.current >= this.end)
    {
      return 'Ready!';
    }

    const totalSeconds = Math.ceil((this.end - this.current) / 1000);

    if(totalSeconds == 1)
      return '1 second' + (this.suffix ? ' ' + this.suffix : '');
    else if(totalSeconds < 60)
      return totalSeconds + ' seconds' + (this.suffix ? ' ' + this.suffix : '');

    let minutes = Math.ceil(totalSeconds / 60);
    let parts = [];

    if(minutes >= 60 * 24)
    {
      const days = Math.floor(minutes / (60 * 24));
      minutes -= days * 60 * 24;
      parts.push(days + ' ' + (days === 1 ? 'day' : 'days'));
    }

    if(minutes >= 60 && parts.length < 2)
    {
      const hours = Math.floor(minutes / 60);
      minutes -= hours * 60;
      parts.push(hours + ' ' + (hours === 1 ? 'hour' : 'hours'));
    }

    if(minutes > 0 && parts.length < 2)
    {
      parts.push(minutes + ' ' + (minutes === 1 ? 'minute' : 'minutes'));
    }

    return parts.join(' ') + (this.suffix ? ' ' + this.suffix : '');
  }

}
