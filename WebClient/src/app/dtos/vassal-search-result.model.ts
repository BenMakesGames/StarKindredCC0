export interface VassalSearchResultModel
{
  id: string;
  name: string;
  portrait: string;
  species: string;
  level: number;
  willpower: number;
  retirementProgress: number;
  favorite: boolean;
  element: string;
  sign: string;
  nature: string;
  recruitDate: string;
  statusEffects: string[];
  tags: { title: string, color: string }[];
  onMission: boolean;
  leader: boolean;
  weapon: { image: string, level: number, primaryBonus: string, secondaryBonus: string|null }|null;
}
