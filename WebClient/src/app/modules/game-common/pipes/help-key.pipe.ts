import { Pipe, PipeTransform } from '@angular/core';

@Pipe({
  name: 'helpKey'
})
export class HelpKeyPipe implements PipeTransform {

  transform(value: string): string {
    return value.toLowerCase()
      // remove non-letters & non-dashes
      .replace(/[^a-z\-]/, '')

      // trim dashes from start and end
      .replace(/^-/, '')
      .replace(/-$/, '')

      // replace sequences of dashes with a single forward slash
      .replace(/-+/, '/')
    ;
  }
}
