import {MissionRewardDto} from "./mission-reward.dto";

export interface CompleteMissionResponseDto
{
  outcome: 'Bad'|'Good'|'Great';
  message: string;
  rewards: MissionRewardDto[];
}
