import { Component, OnDestroy, OnInit } from '@angular/core';
import {ApiService} from "../../../base/services/api.service";
import {interval, Subscription} from "rxjs";
import { AllianceMemberDialog } from "../../dialogs/alliance-member/alliance-member.dialog";
import { MatDialog } from "@angular/material/dialog";
import {RecruitDialog} from "../../dialogs/recruit/recruit.dialog";
import {HSL} from "../../../../dtos/hsl.model";
import {EnterInviteCodeDialog} from "../../dialogs/enter-invite-code/enter-invite-code.dialog";

@Component({
  selector: 'app-alliance',
  templateUrl: './alliance.component.html',
  styleUrls: ['./alliance.component.scss']
})
export class AllianceComponent implements OnInit, OnDestroy {

  loadAllianceSubscription = Subscription.EMPTY;
  clockSubscription = Subscription.EMPTY;

  alliance: AllianceDto|null = null;
  leader: AllianceMemberDto|null = null;

  giantTimeUntil: number = 0;
  giantTimeRemaining: number = 0;

  constructor(private api: ApiService, private matDialog: MatDialog) { }

  ngOnInit(): void
  {
    this.loadAlliance();
  }

  ngOnDestroy()
  {
    this.clockSubscription.unsubscribe();
    this.loadAllianceSubscription.unsubscribe();
  }

  loadAlliance()
  {
    this.loadAllianceSubscription.unsubscribe();

    this.loadAllianceSubscription = this.api.get<AllianceDto>('alliances/my').subscribe({
      next: r => {
        this.alliance = r.data;

        if(this.alliance)
        {
          this.startGiantTimer();

          this.leader = this.alliance.members.find(m => m.id == this.alliance!.leaderId)!;
          this.alliance.members = this.alliance.members.sort((a, b) => {
            if(b.rankLevel == a.rankLevel)
            {
              if(b.rank?.toLowerCase() == a.rank?.toLowerCase())
                return b.level - a.level;
              else
              {
                if(a.rank == null)
                  return 1;
                else if(b.rank == null)
                  return -1;
                else
                  return a.rank.toLowerCase().localeCompare(b.rank.toLowerCase());
              }
            }
            else
              return b.rankLevel - a.rankLevel;
          });
        }
      },
    });
  }

  doViewMember(member: AllianceMemberDto)
  {
    if(!this.alliance)
      return;

    AllianceMemberDialog.open(this.matDialog, this.alliance.rights, this.alliance.myId, member.id, member.name, member.rank, member.avatar, member.color).afterClosed().subscribe({
      next: r => {
        if(!r) return;

        if(r.changedTitle)
        {
          this.alliance = {
            ...this.alliance!,
            members: this.alliance!.members.map(m => {
              if(m.id == member.id)
              {
                return {
                  ...m,
                  rank: r.changedTitle,
                };
              }
              else
                return m;
            })
          };
        }
        else if(r.kicked)
        {
          this.alliance = {
            ...this.alliance!,
            members: this.alliance!.members.filter(m => m.id != member.id),
          };
        }
        else if(r.left)
        {
          this.alliance = null;
        }
        else if(r.changedAppearance)
        {
          this.alliance = {
            ...this.alliance!,
            members: this.alliance!.members.map(m => {
              if(m.id == member.id)
              {
                return {
                  ...m,
                  avatar: r.changedAppearance.avatar ?? m.avatar,
                  color: r.changedAppearance.color ?? m.color
                };
              }
              else
                return m;
            })
          };
        }
      }
    });
  }

  doTrackAGiant()
  {
    this.api.post<GiantDto>('alliances/trackGiant').subscribe({
      next: r => {
        if(!this.alliance)
          return;

        this.alliance = {
          ...this.alliance,
          giant: r.data
        };

        this.startGiantTimer();
      }
    });
  }

  doCompleteGiant()
  {
    if(!this.alliance?.giant)
      return;

    this.api.post('alliances/trackGiant').subscribe({
      next: () => {
        this.loadAlliance();
      }
    });
  }

  private startGiantTimer()
  {
    this.updateNow();

    this.clockSubscription.unsubscribe();
    this.clockSubscription = interval(2000).subscribe({
      next: () => {
        this.updateNow();
      }
    });
  }

  private updateNow()
  {
    const now = Date.now();

    if(this.alliance?.giant)
    {
      this.giantTimeUntil = Date.parse(this.alliance.giant.startsOn) - now;
      this.giantTimeRemaining = Date.parse(this.alliance.giant.expiresOn) - now;
    }
  }

  doRecruit()
  {
    RecruitDialog.open(this.matDialog).afterClosed().subscribe({
      next: recruited => {
        if(recruited)
          this.loadAlliance();
      }
    });
  }

  doEnterCode()
  {
    EnterInviteCodeDialog.open(this.matDialog).afterClosed().subscribe({
      next: allianceId => {
        if(!allianceId) return;

        this.loadAlliance();
      }
    });
  }
}

interface AllianceDto {
  members: AllianceMemberDto[];
  leaderId: string;
  myId: string;
  createdOn: string;
  level: number;
  giant: GiantDto|null;
  rights: string[];
  logs: LogDto[];
}

interface AllianceMemberDto {
  id: string;
  name: string;
  level: number;
  rank: string|null;
  rankLevel: number;
  avatar: string;
  color: HSL;
}

interface GiantDto {
  startsOn: string;
  expiresOn: string;
  element: string;
  damage: number;
  health: number;
}

interface LogDto
{
  createdOn: string;
  activityType: string;
  message: string;
}
