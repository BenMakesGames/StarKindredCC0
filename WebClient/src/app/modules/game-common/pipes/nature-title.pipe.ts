import { Pipe, PipeTransform } from '@angular/core';

@Pipe({
  name: 'natureTitle'
})
export class NatureTitlePipe implements PipeTransform {

  transform(value: string): string {
    switch(value)
    {
      case 'ThrillSeeker': return 'Thrill-seeker';
      default: return value;
    }
  }
}
