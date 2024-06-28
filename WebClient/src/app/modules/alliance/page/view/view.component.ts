import {Component, OnDestroy, OnInit} from '@angular/core';
import {ActivatedRoute, Router} from "@angular/router";
import {Subscription} from "rxjs";
import {ApiService} from "../../../base/services/api.service";
import {HSL} from "../../../../dtos/hsl.model";

@Component({
  templateUrl: './view.component.html',
  styleUrls: ['./view.component.scss']
})
export class ViewComponent implements OnInit, OnDestroy {

  paramSubscription = Subscription.EMPTY;
  allianceId: string|null = null;
  alliance: AllianceDto|null = null;
  leader: MemberDto|null = null;

  joining = false;

  constructor(private activatedRoute: ActivatedRoute, private api: ApiService, private router: Router) { }

  ngOnInit(): void {
    this.paramSubscription = this.activatedRoute.paramMap.subscribe(params => {
      this.allianceId = params.get('id')!;

      this.loadAlliance();
    });
  }

  ngOnDestroy() {
    this.paramSubscription.unsubscribe();
  }

  doJoin()
  {
    if(this.joining) return;

    this.joining = true;

    this.api.post('alliances/' + this.allianceId + '/join').subscribe({
      next: _ => {
        // noinspection JSIgnoredPromiseFromCall
        this.router.navigateByUrl('/alliance');
      },
      error: () => {
        this.joining = false;
      }
    })
  }

  loadAlliance()
  {
    this.api.get<AllianceDto>('alliances/' + this.allianceId).subscribe({
      next: r => {
        this.alliance = r.data;
        this.leader = this.alliance.members.find(m => m.id == this.alliance!.leaderId)!;
        this.alliance.members = this.alliance.members.sort((a, b) => {
          if(b.rankLevel == a.rankLevel)
            return b.level - a.level;
          else
            return b.rankLevel - a.rankLevel;
        });
      }
    });
  }
}

interface AllianceDto
{
  leaderId: string;
  createdOn: string;
  level: number;
  members: MemberDto[];
  openInvitation: { minLevel: number, maxLevel: number }|null;
}

interface MemberDto {
  id: string;
  name: string;
  avatar: string;
  color: HSL;
  level: number;
  rankLevel: number;
}
