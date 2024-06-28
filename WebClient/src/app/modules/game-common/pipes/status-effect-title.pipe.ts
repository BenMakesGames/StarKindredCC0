import { Pipe, PipeTransform } from '@angular/core';

@Pipe({
  name: 'statusEffectTitle'
})
export class StatusEffectTitlePipe implements PipeTransform {

  transform(value: string): string {
    switch(value)
    {
      case 'BrokenBone': return 'Broken Bone';
      case 'Focused': return 'Focused';
      case 'OracleTimeout': return 'Suspended';
      case 'GoldenTouch': return 'Golden Touch';
      case 'ArtisticVision': return 'Artistic Vision';
      case 'Power': return 'Strength';
      default: return '???';
    }
  }

}
