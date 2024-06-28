import { Pipe, PipeTransform } from '@angular/core';
import { DescribePrimaryBonusPipe } from "./describe-primary-bonus.pipe";

@Pipe({
  name: 'describeSecondaryBonus'
})
export class DescribeSecondaryBonusPipe implements PipeTransform {

  transform(bonus: string, level: number): string {
    return DescribePrimaryBonusPipe.describe(bonus, level == 5 ? 1 : 0);
  }
}
