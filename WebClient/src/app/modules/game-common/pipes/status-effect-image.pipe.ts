import { Pipe, PipeTransform } from '@angular/core';

@Pipe({
  name: 'statusEffectImage'
})
export class StatusEffectImagePipe implements PipeTransform {

  transform(value: string): string {
    return '/assets/status-effects/' + value.toLowerCase() + '.svg';
  }
}
