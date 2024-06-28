import { ResourceQuantityDto } from "./resource-quantity.dto";

export interface BuildingDto
{
  id: string;
  position: number;
  type: string;
  level: number;
  maxLevel: number;
  yieldProgress: number;
  secondsRequired: number;
  yield: ResourceQuantityDto[];
  upgradeCost: ResourceQuantityDto[];
  availableSpecializations: string[];
  left: number;
  top: number;
  powersAvailableOn: string|null;
  powersAvailable: PowerDto[]|null;
}

export interface PowerDto
{
  power: string;
  title: string;
  image: string;
  cost: ResourceQuantityDto|null;
}
