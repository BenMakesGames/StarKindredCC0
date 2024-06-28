import {Component, ElementRef, HostListener, OnDestroy, OnInit, ViewChild} from '@angular/core';
import {ApiService} from "../../../base/services/api.service";
import {interval, Observable, Subject, Subscription} from "rxjs";
import {TownResourcesService} from "../../../base/services/town-resources.service";
import {ResourceQuantityDto} from "../../../../dtos/resource-quantity.dto";
import {ActivatedRoute} from "@angular/router";
import {DoStoryDialog} from "../../dialogs/do-story/do-story.dialog";
import {MatDialog} from "@angular/material/dialog";
import {AvailableStoryStepModel} from "../../models/available-story-step.model";
import {MissionResultsDialog} from "../../../home/dialogs/mission-results/mission-results.dialog";
import {ShowCompletedStoryDialog} from "../../dialogs/show-completed-story/show-completed-story.dialog";
import {YesOrNoDialog} from "../../../base/dialogs/yes-or-no/yes-or-no.dialog";
import {MissionRewardDto} from "../../../../dtos/mission-reward.dto";

@Component({
  templateUrl: './view.component.html',
  styleUrls: ['./view.component.scss']
})
export class ViewComponent implements OnInit, OnDestroy {

  @ViewChild('Spotlight', { static: false }) spotlight: ElementRef|undefined;

  @HostListener('document:mousemove', ['$event'])
  mouseMove(event: any) {
    this.moveSpotlight(event.clientX, event.clientY);
  }

  @HostListener('document:touchstart', ['$event'])
  touchStart(event: any) {
    this.moveSpotlight(event.touches[0].clientX, event.touches[0].clientY);
  }

  @HostListener('document:touchmove', ['$event'])
  touchMove(event: any) {
    this.moveSpotlight(event.touches[0].clientX, event.touches[0].clientY);
  }

  moveSpotlight(x: number, y: number)
  {
    if(this.spotlight) {
      this.spotlight.nativeElement.style.maskImage = `radial-gradient(min(max(25vw, 25vh), 200px) at ${x}px ${y}px, transparent 100%, black 100%)`;
      this.spotlight.nativeElement.style['-webkit-mask-image'] = `radial-gradient(min(max(25vw, 25vh), 200px) at ${x}px ${y}px, transparent 100%, black 100%)`;
    }
  }

  resources: ResourceQuantityDto[]|null = null;

  resourcesSubscription = Subscription.EMPTY;
  paramSubscription = Subscription.EMPTY;
  storySubscription = Subscription.EMPTY;
  timeSubscription = Subscription.EMPTY;

  storyId: string|null = null;
  story: StoryDto|null = null;
  now: number;

  inProgressSteps: InProgressStep[] = [];
  availableSteps: AvailableStoryStepModel[] = [];

  completingAMission = false;

  constructor(
    private api: ApiService, private town: TownResourcesService, private activatedRoute: ActivatedRoute,
    private matDialog: MatDialog
  ) {
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

    this.paramSubscription = this.activatedRoute.paramMap.subscribe(params => {
      this.storyId = params.get('storyId');

      this.loadStory();
    });

    this.timeSubscription = interval(2000).subscribe({
      next: () => {
        this.now = Date.now();
      }
    });
  }

  ngOnDestroy()
  {
    this.resourcesSubscription.unsubscribe();
    this.storySubscription.unsubscribe();
    this.paramSubscription.unsubscribe();
    this.timeSubscription.unsubscribe();
  }

  doAssignToMission(mission: AvailableStoryStepModel)
  {
    if(!this.story) return;

    DoStoryDialog.open(this.matDialog, this.story.tags, mission).afterClosed().subscribe({
      next: started => {
        if(started)
          this.loadStory();
      }
    })
  }

  doAbort(step: InProgressStep)
  {
    if(this.completingAMission)
      return;

    YesOrNoDialog.open(
      this.matDialog,
      () => this.doReallyAbort('stories/' + step.inProgress.id + '/abort'),
      'Abort Mission'
    )
      .afterClosed().subscribe({
        next: confirmed => {
          if(confirmed)
            this.loadStory();
        }
      })
    ;
  }

  doReallyAbort(endpoint: string): Observable<boolean>
  {
    const result = new Subject<boolean>();

    this.api.post(endpoint).subscribe({
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

  doComplete(step: InProgressStep)
  {
    if(this.completingAMission) return;

    this.completingAMission = true;

    this.api.post<{ text: string, rewards: MissionRewardDto[] }>('stories/' + step.inProgress.id + '/complete').subscribe({
      next: r => {
        MissionResultsDialog.open(this.matDialog, {
          outcome: 'Good',
          message: r.data.text,
          rewards: r.data.rewards
        });
        this.completingAMission = false;
        this.town.reload();
        this.loadStory();
      },
      error: () => {
        this.completingAMission = false;
      }
    })
  }

  doShowStory(story: CompletedDto)
  {
    ShowCompletedStoryDialog.open(this.matDialog, story.id);
  }

  loadStory()
  {
    this.storySubscription.unsubscribe();

    this.storySubscription = this.api.get<StoryDto>('stories/' + this.storyId).subscribe({
      next: r => {
        this.story = r.data;

        this.inProgressSteps = this.story.inProgress.map(p => {
          return {
            inProgress: p,
            availableStep: this.addPinDirection(this.story!.availableSteps.find(s => s.step == p.step)!)
          }
        });

        this.availableSteps = this.story.availableSteps
          .filter(a => !this.story!.inProgress.some(p => p.step == a.step))
          .map(a => this.addPinDirection(a))
        ;
      }
    });
  }

  addPinDirection(s: AvailableStoryStepModel)
  {
    if(s.pinOverride)
      return s;
    else
      return { ...s, pinOverride: this.pinDirection(s.x, s.y) };
  }

  pinDirection(x: number, y: number): string
  {
    if(x < 15)
      return 'Left';

    if(x >= 85)
      return 'Right';

    if(y >= 50)
      return 'Bottom';

    return 'Top';
  }
}

interface InProgressStep
{
  inProgress: InProgressDto;
  availableStep: AvailableStoryStepModel;
}

interface StoryDto
{
  isDark: boolean;
  year: number;
  month: number;
  tags: TagDto[];
  completed: CompletedDto[];
  availableSteps: AvailableStoryStepModel[];
  inProgress: InProgressDto[];
}

interface CompletedDto
{
  id: string;
  step: number;
  x: number;
  y: number;
  type: string;
}

interface InProgressDto
{
  id: string;
  step: number;
  startedOn: number;
  completesOn: number;
  vassals: VassalDto[];
}

interface VassalDto
{
  id: string;
  level: number;
  element: string;
  species: string;
  portrait: string;
}

interface TagDto
{
  title: string;
  color: string;
}
