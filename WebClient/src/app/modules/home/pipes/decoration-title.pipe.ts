import { Pipe, PipeTransform } from '@angular/core';

@Pipe({
  name: 'decorationTitle'
})
export class DecorationTitlePipe implements PipeTransform {

  transform(value: string): string {
    switch(value)
    {
      case 'LogPile': return 'Log Pile';
      case 'RedFlag': return 'Red Flag';
      case 'BlueFlag': return 'Blue Flag';
      case 'WhiteFlag': return 'White Flag';
      case 'PurpleFlag': return 'Purple Flag';
      case 'BlackFlag': return 'Black Flag';
      case 'FalseAveries': return 'False Averies';
      case 'MarbleHead': return 'Marble Head';
      case 'ShalurianLighthouse': return 'Shalurian Lighthouse';
      case 'VanillaIceCream': return 'Vanilla Ice Cream';
      case 'ChocolateIceCream': return 'Chocolate Ice Cream';
      case 'VanillaIceCreamWithCherry': return 'Vanilla Ice Cream w/ Cherry';
      case 'ChocolateIceCreamWithCherry': return 'Chocolate Ice Cream w/ Cherry';
      case 'SmallMushrooms': return 'Small Mushrooms';
      case 'LargeMushroom': return 'Large Mushroom';
      case 'SkeletalRemains': return 'Skeletal Remains';
      case 'FenceNorthSouth': return 'Fence (N-S)';
      case 'FenceEastWest': return 'Fence (E-W)';
      case 'EnormousTibia': return 'Enormous Tibia';
      case 'PurpleGrass': return 'Purple Grass';
      case 'WoodenBridge': return 'Wooden Bridge';
      case 'StoneBridge': return 'Stone Bridge';
      case 'PalaceTower': return 'Palace Tower';
      case 'SwordInStone': return 'A Sword in Stone';
      default: return value;
    }
  }
}
