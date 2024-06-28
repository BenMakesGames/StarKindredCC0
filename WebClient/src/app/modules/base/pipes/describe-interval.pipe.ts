import { Pipe, PipeTransform } from '@angular/core';

@Pipe({
  name: 'describeInterval'
})
export class DescribeIntervalPipe implements PipeTransform {

  transform(ms: number): string {
    let seconds = Math.ceil(ms / 1000);
    let parts = [];

    if(seconds > 60 * 60 * 24)
    {
      const days = Math.floor(seconds / (60 * 60 * 24));
      parts.push(days + ' ' + (days == 1 ? 'day' : 'days'));
      seconds = seconds % (60 * 60 * 24);
    }

    if(seconds > 60 * 60 && parts.length < 2)
    {
      const hours = Math.floor(seconds / (60 * 60));
      parts.push(hours + ' ' + (hours == 1 ? 'hour' : 'hours'));
      seconds = seconds % (60 * 60);
    }

    if(seconds > 60 && parts.length < 2)
    {
      const minutes = Math.floor(seconds / 60);
      parts.push(minutes + ' ' + (minutes == 1 ? 'minute' : 'minutes'));
      seconds = seconds % 60;
    }

    if(parts.length < 2)
      parts.push(seconds + ' ' + (seconds == 1 ? 'second' : 'seconds'));

    return parts.join(', ');
  }
}
