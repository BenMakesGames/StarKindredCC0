export interface AvailableStoryStepModel
{
  id: string;
  step: number;
  x: number;
  y: number;
  pinOverride: string|null;
  type: string;
  title: string;
  durationInMinutes: number;
  minVassals: number;
  maxVassals: number;
  requiredElement: string|null;
}
