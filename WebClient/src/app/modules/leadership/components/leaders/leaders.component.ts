import {Component, OnDestroy, OnInit} from '@angular/core';
import {ApiService} from "../../../base/services/api.service";
import {AssignVassalToPositionDialog} from "../../dialogs/assign-vassal-to-position/assign-vassal-to-position.dialog";
import {MatDialog} from "@angular/material/dialog";
import {interval, Subject, Subscription} from "rxjs";
import {YesOrNoDialog} from "../../../base/dialogs/yes-or-no/yes-or-no.dialog";
import {ResearchDialog} from "../../dialogs/research/research.dialog";
import {ResourceQuantityDto} from "../../../../dtos/resource-quantity.dto";
import {TownResourcesService} from "../../../base/services/town-resources.service";
import {HelpDialog} from "../../../game-common/dialogs/help/help.dialog";

@Component({
  selector: 'app-leaders',
  templateUrl: './leaders.component.html',
  styleUrls: ['./leaders.component.scss']
})
export class LeadersComponent implements OnInit, OnDestroy {

  waitingOnAPI = false;

  positions = [ 'Cosmology', 'Culture', 'Defense', 'Economy' ];

  resources: ResourceQuantityDto[]|null = null;
  leadersByPosition: {[key: string]: LeaderDto}|null = null;
  projectsByPosition: {[key: string]: ProjectProgress|null} = {};
  establishTimes: {[key: string]: number} = {};
  tags: { title: string, color: string }[] = [];
  now: number = Date.now();
  nowSubscription = Subscription.EMPTY;
  resourcesSubscription = Subscription.EMPTY;
  completedProjects: CompletedProject[]|null = null;

  constructor(private api: ApiService, private matDialog: MatDialog, private town: TownResourcesService) { }

  ngOnInit(): void {
    this.resourcesSubscription = this.town.resources.subscribe({
      next: r => {
        if(r)
          this.resources = r;
        else
          this.town.reload();
      }
    });

    this.api.get<{ tags: { title: string, color: string }[] }>(`accounts/tags`).subscribe({
      next: r => {
        this.tags = r.data.tags;
      }
    });

    this.nowSubscription = interval(2000).subscribe(() => {
      this.now = Date.now();
    });

    this.loadLeaders();
    this.loadCompletedProjects();
  }

  private loadCompletedProjects()
  {
    this.api.get<{ projects: CompletedProject[] }>('leaders/completedProjects').subscribe({
      next: r => {
        this.completedProjects = r.data.projects;
        this.doSortCompletedProjects();
      }
    });
  }

  ngOnDestroy() {
    this.nowSubscription.unsubscribe();
    this.resourcesSubscription.unsubscribe();
  }

  private loadLeaders()
  {
    this.api.get<{ leaders: LeaderDto[] }>('leaders').subscribe({
      next: r => {
        this.leadersByPosition = {};

        for(let l of r.data.leaders)
        {
          this.leadersByPosition[l.position] = l;
          this.establishTimes[l.position] = Date.parse(l.establishedOn);
          this.projectsByPosition[l.position] = this.createProjectData(l.project);
        }

        console.log(this.projectsByPosition);
      }
    });
  }

  private createProjectData(project: ProjectDto|null)
  {
    if(project == null)
      return null;

    const startedOn = Date.parse(project.startDate);
    const endsOn = Date.parse(project.endDate);

    return {
      technology: project.technology,
      title: project.title,
      startedOn: startedOn,
      endsOn: endsOn,
      duration: endsOn - startedOn
    };
  }

  doAssignLeader(position: string)
  {
    if(this.waitingOnAPI) return;

    AssignVassalToPositionDialog.open(this.matDialog, this.tags, position).afterClosed().subscribe({
      next: vassal => {
        if(vassal)
        {
          this.loadLeaders();
        }

        this.waitingOnAPI = false;
      }
    })
  }

  doRemoveLeader(position: string) {
    if (this.waitingOnAPI) return;

    YesOrNoDialog.open(this.matDialog, () => this.reallyRemoveLeader(position), 'Remove Leader');
  }

  doCompleteResearch(leader: LeaderDto)
  {
    if(this.waitingOnAPI || leader.project == null)
      return;

    this.waitingOnAPI = true;

    this.api.post('leaders/' + leader.position + '/completeResearch').subscribe({
      next: () => {
        HelpDialog.open(this.matDialog, 'technology-' + leader.project!.technology, 'Research Complete!');

        leader.project = null;
        delete this.projectsByPosition[leader.position];

        this.waitingOnAPI = false;

        this.loadCompletedProjects();
      },
      error: () => {
        this.waitingOnAPI = false;
      }
    })
  }

  doCancelResearch(leader: LeaderDto) {
    if (this.waitingOnAPI)
      return;

    YesOrNoDialog.open(this.matDialog, () => this.reallyCancelResearch(leader), 'Cancel Project?', 'You will receive a full refund on the resources spent.');
  }

  reallyCancelResearch(leader: LeaderDto)
  {
    const result = new Subject<boolean>();

    this.waitingOnAPI = true;

    this.api.post('leaders/' + leader.position + '/cancelResearch').subscribe({
      next: () => {
        leader.project = null;
        delete this.projectsByPosition[leader.position];
        this.town.reload();

        this.waitingOnAPI = false;
        result.next(true);
        result.complete();
      },
      error: () => {
        this.waitingOnAPI = false;
        result.next(false);
        result.complete();
      }
    });

    return result;
  }

  doChooseResearch(position: string)
  {
    ResearchDialog.open(this.matDialog, position).afterClosed().subscribe({
      next: r => {
        if(r && r.cost)
        {
          this.loadLeaders();
          this.town.spend(r.cost);
        }
      }
    });
  }

  private reallyRemoveLeader(position: string)
  {
    const result = new Subject<boolean>();

    this.waitingOnAPI = true;

    this.api.post('leaders/' + this.leadersByPosition![position].vassal.id + '/remove').subscribe({
      next: _ => {
        delete this.leadersByPosition![position];
        this.waitingOnAPI = false;
        result.next(true);
        result.complete();
      },
      error: () => {
        this.waitingOnAPI = false;
        result.next(false);
        result.complete();
      }
    });

    return result;
  }

  public currentSort = {
    property: 'completedOn',
    direction: 'desc'
  };

  public doClickSortHeader(propertyName: string, defaultDirection: 'asc'|'desc')
  {
    if(this.currentSort.property == propertyName)
    {
      this.currentSort = {
        ...this.currentSort,
        direction: this.currentSort.direction === 'desc' ? 'asc' : 'desc'
      };
    }
    else
    {
      this.currentSort = {
        property: propertyName,
        direction: defaultDirection
      };
    }

    this.doSortCompletedProjects();
  }

  public doSortCompletedProjects()
  {
    if(!this.completedProjects)
      return;

    this.completedProjects = this.completedProjects.sort((a: any, b: any) => {
      if(this.currentSort.direction === 'desc')
        return b[this.currentSort.property].localeCompare(a[this.currentSort.property]);
      else
        return a[this.currentSort.property].localeCompare(b[this.currentSort.property]);
    });
  }

  public doShowProjectHelp(e: Event, technology: string)
  {
    e.stopPropagation();
    HelpDialog.open(this.matDialog, 'technology-' + technology.toLowerCase());
  }
}

interface LeaderDto {
  position: string;
  establishedOn: string;
  vassal: LeaderVassalDto;
  project: ProjectDto|null;
}

interface LeaderVassalDto
{
  id: string,
  element: string;
  portrait: string;
  species: string;
  level: number;
  name: string;
  nature: string;
  sign: string;
}

interface ProjectDto
{
  technology: string;
  title: string;
  startDate: string;
  endDate: string;
}

interface ProjectProgress
{
  technology: string;
  title: string;
  startedOn: number;
  endsOn: number;
  duration: number;
}

interface CompletedProject
{
  technology: string;
  title: string;
  completedOn: string;
}
