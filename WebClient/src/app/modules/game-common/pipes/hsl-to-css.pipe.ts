import { Pipe, PipeTransform } from '@angular/core';
import {HSL} from "../../../dtos/hsl.model";

@Pipe({
  name: 'hslToCss'
})
export class HslToCssPipe implements PipeTransform {

  transform(value: HSL): string {
    return 'hsl(' + value.hue + ',' + value.saturation + '%,' + value.luminosity + '%)';
  }
}
