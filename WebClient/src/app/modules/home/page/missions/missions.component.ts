import { Component, OnDestroy, OnInit } from '@angular/core';
import { ApiService } from "../../../base/services/api.service";
import {GoOnMissionDialog} from "../../dialogs/go-on-mission/go-on-mission.dialog";
import {MatDialog} from "@angular/material/dialog";
import {MissionResultsDialog} from "../../dialogs/mission-results/mission-results.dialog";
import {CompleteMissionResponseDto} from "../../../../dtos/complete-mission-response.dto";
import { interval, Observable, Subject, Subscription } from "rxjs";
import {GoOnGenericHuntMissionDialog} from "../../dialogs/go-on-generic-hunt-mission/go-on-generic-hunt-mission.dialog";
import {GoOnTimedMissionDialog} from "../../dialogs/go-on-timed-mission/go-on-timed-mission.dialog";
import { ResourceQuantityDto } from "../../../../dtos/resource-quantity.dto";
import { TownResourcesService } from "../../../base/services/town-resources.service";
import {FightGiantDialog} from "../../dialogs/fight-giant/fight-giant.dialog";
import {YesOrNoDialog} from "../../../base/dialogs/yes-or-no/yes-or-no.dialog";

@Component({
  templateUrl: './missions.component.html',
  styleUrls: ['./missions.component.scss']
})
export class MissionsComponent implements OnInit, OnDestroy {

  resourcesSubscription = Subscription.EMPTY;
  resources: ResourceQuantityDto[]|null = null;

  clockSubscription = Subscription.EMPTY;
  response: Response|null = null;
  now: number;
  giantTimeUntil: number = 0;
  giantTimeRemaining: number = 0;

  completingAMission = false;

  missionCoordinates: {[key:string]: {x: number, y: number, pin:'top'|'bottom' }} = {
    RecruitTown: { x: 19, y: 17.5, pin: 'bottom' },
    Oracle: { x: 32.9, y: 4, pin: 'top' },
    HuntLevel0: { x: 8, y: 23, pin: 'top' },
    HuntLevel10: { x: 56, y: 22, pin: 'bottom' },
    HuntLevel20: { x: 33, y: 65.5, pin: 'bottom' },
    HuntLevel50: { x: 44, y: 42, pin: 'bottom' },
    HuntLevel80: { x: 19, y: 75, pin: 'bottom' },
    HuntLevel120: { x: 78, y: 46, pin: 'bottom' },
    HuntLevel200: { x: 70, y: 80, pin: 'top' },
  };

  timedMissionPositions = [
    // land: 0-9
    { left: 25, top: 31.5, pin: 'top' },
    { left: 59, top: 30, pin: 'top' },
    { left: 80, top: 2, pin: 'top' },
    { left: 10, top: 41, pin: 'top' },
    { left: 12, top: 78.5, pin: 'top' },
    { left: 25.5, top: 97, pin: 'bottom' },
    { left: 42, top: 78, pin: 'top' },
    { left: 90, top: 39, pin: 'top' },
    { left: 75, top: 78, pin: 'bottom' },
    { left: 50, top: 55, pin: 'top' },

    // sea: 10, 11, 12
    { left: 23, top: 17, pin: 'top' },
    { left: 70, top: 45, pin: 'top' },
    { left: 34, top: 77, pin: 'bottom' },
  ];

  constructor(private api: ApiService, private matDialog: MatDialog, private town: TownResourcesService) {
    this.now = Date.now();
  }

  ngOnInit(): void {
    this.resourcesSubscription = this.town.resources.subscribe({
      next: r => {
        if(r)
          this.resources = r;
        else
          this.town.reload();
      }
    });

    this.loadMissions();
  }

  ngOnDestroy()
  {
    this.clockSubscription.unsubscribe();
    this.resourcesSubscription.unsubscribe();
  }

  private loadMissions()
  {
    this.api.get<Response>('missions').subscribe({
      next: r => {
        this.response = r.data;

        // depends on this.response being set:
        this.updateNow();

        // sort vassals by level:
        for(let m of this.response.missions)
          m.vassals.sort((a, b) => b.level - a.level);

        for(let m of this.response.timedMissions)
          m.vassals.sort((a, b) => b.level - a.level);

        this.clockSubscription.unsubscribe();
        this.clockSubscription = interval(2000).subscribe({
          next: () => {
            this.updateNow();
          }
        });
      }
    });
  }

  private updateNow()
  {
    this.now = Date.now();

    if(this.response?.giant)
    {
      this.giantTimeUntil = Date.parse(this.response.giant.startsOn) - this.now;
      this.giantTimeRemaining = Date.parse(this.response.giant.expiresOn) - this.now;
    }
  }

  doAssignToMission(mission: string, maxVassals: number)
  {
    if(!this.response || this.completingAMission)
      return;

    if(mission === 'Giant')
    {
      FightGiantDialog.open(this.matDialog, this.response.tags, this.response.giant!.element).afterClosed().subscribe({
        next: went => {
          if(went)
            this.loadMissions();
        }
      });
    }
    else if(mission.startsWith('HuntLevel'))
    {
      const level = parseInt(mission.substring(9));

      GoOnGenericHuntMissionDialog.open(this.matDialog, this.response.tags, mission, level, maxVassals).afterClosed().subscribe({
        next: went => {
          if(went)
            this.loadMissions();
        }
      });
    }
    else
    {
      GoOnMissionDialog.open(this.matDialog, this.response.tags, mission, maxVassals).afterClosed().subscribe({
        next: went => {
          if(went)
            this.loadMissions();
        }
      });
    }
  }

  doAssignToTimedMission(mission: string, missionId: string, level: number, element: string|null, minVassals: number, maxVassals: number)
  {
    if(!this.response || this.completingAMission)
      return;

    GoOnTimedMissionDialog.open(this.matDialog, this.response.tags, mission, missionId, level, element, minVassals, maxVassals).afterClosed().subscribe({
      next: went => {
        if(went)
          this.loadMissions();
      }
    });
  }

  doComplete(mission: MissionDto)
  {
    if(this.completingAMission)
      return;

    this.completingAMission = true;

    this.api.post<CompleteMissionResponseDto>('missions/' + mission.id + '/complete').subscribe({
      next: r => {
        MissionResultsDialog.open(this.matDialog, r.data);
        this.town.reload();
        this.loadMissions();
        this.completingAMission = false;
      },
      error: () => {
        this.completingAMission = false;
      }
    });
  }

  doCompleteTimedMission(mission: TimedMissionDto)
  {
    if(this.completingAMission)
      return;

    this.completingAMission = true;

    this.api.post<CompleteMissionResponseDto>('timedMissions/' + mission.id + '/complete').subscribe({
      next: r => {
        MissionResultsDialog.open(this.matDialog, r.data);
        this.town.reload();
        this.loadMissions();
        this.completingAMission = false;
      },
      error: () => {
        this.completingAMission = false;
      }
    });
  }

  doAbort(mission: MissionDto)
  {
    if(this.completingAMission)
      return;

    YesOrNoDialog.open(
      this.matDialog,
      () => this.doReallyAbort('missions/' + mission.id),
      'Abort Mission'
    )
      .afterClosed().subscribe({
        next: confirmed => {
          if(confirmed)
            this.loadMissions();
        }
      })
    ;
  }

  doAbortTimedMission(mission: TimedMissionDto)
  {
    if(this.completingAMission)
      return;

    YesOrNoDialog.open(
      this.matDialog,
      () => this.doReallyAbort('timedMissions/' + mission.id),
      'Abort Mission'
    )
      .afterClosed().subscribe({
        next: confirmed => {
          if(confirmed)
            this.loadMissions();
        }
      })
    ;
  }

  doReallyAbort(endpoint: string): Observable<boolean>
  {
    const result = new Subject<boolean>();

    this.api.post(endpoint + '/abort').subscribe({
      next: _ => {
        result.next(true);
        result.complete();
      },
      error: () => {
        result.next(false);
        result.complete();
      }
    });

    return result;
  }
}

interface Response
{
  tags: TagDto[];
  missions: MissionDto[];
  available: AvailableDto[];
  timedMissions: TimedMissionDto[];
  giant: GiantDto|null;
}

interface TimedMissionDto
{
  id: string;
  type: string;
  element: string|null;
  location: number;
  level: number;
  species: string|null;
  treasure: string|null;
  weapon: string|null;
  vassals: VassalDto[];
  minVassals: number;
  maxVassals: number;
  startedOn: number|null;
  completesOn: number|null;
}

interface AvailableDto
{
  type: string;
  minVassals: number;
  maxVassals: number;
}

interface MissionDto
{
  id: string;
  type: string;
  vassals: VassalDto[];
  startedOn: number;
  completesOn: number;
}

interface VassalDto
{
  id: string;
  portrait: string;
  species: string;
  name: string;
  level: number;
  element: string;
}

interface TagDto
{
  title: string;
  color: string;
}

interface GiantDto
{
  element: string;
  level: number;
  health: number;
  damage: number;
  startsOn: string;
  expiresOn: string;
  canAttack: boolean;
}
