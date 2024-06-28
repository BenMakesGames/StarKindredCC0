import {BuildingDto} from "./building.dto";
import {ResourceQuantityDto} from "./resource-quantity.dto";

export interface TownDto
{
  name: string;
  level: number;
  rumorWaiting: boolean;
  canDecorate: boolean;
  maxDecorations: number;
  buildings: BuildingDto[];
  decorations: DecorationDto[];
  resources: ResourceQuantityDto[];
  goodies: GoodieDto[];
}

export interface GoodieDto
{
  position: number;
  type: string;
  quantity: number;
}

export interface DecorationDto
{
  type: string;
  x: number; // float % value, ex: 63.15
  y: number;
  scale: number; // integer % value, ex: 120
  flipX: boolean;
}
