import {Component, OnDestroy, OnInit} from '@angular/core';
import { ApiService } from "../../../base/services/api.service";
import { MatDialog } from "@angular/material/dialog";
import { BuildDialog } from "../../dialogs/build/build.dialog";
import {ResourceQuantityDto} from "../../../../dtos/resource-quantity.dto";
import {interval, Subscription} from "rxjs";
import { UpgradeDialog } from "../../dialogs/upgrade/upgrade.dialog";
import { BuildingDto } from "../../../../dtos/building.dto";
import {OkDialog} from "../../../game-common/dialogs/ok/ok.dialog";
import { TownResourcesService } from "../../../base/services/town-resources.service";
import { SpecializeDialog } from "../../dialogs/specialize/specialize.dialog";
import {TownPositions} from "../../helpers/town-positions";
import {RenameTownDialog} from "../../dialogs/rename-town/rename-town.dialog";
import {GoodieDto, TownDto} from "../../../../dtos/town.dto";
import {UseBuildingPowerDialog} from "../../dialogs/use-building-power/use-building-power.dialog";

@Component({
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.scss']
})
export class HomeComponent implements OnInit, OnDestroy {

  static readonly secondsBetweenTick = 2;

  TownPositions = TownPositions;
  clockSubscription = Subscription.EMPTY;

  constructor(private api: ApiService, private matDialog: MatDialog, private townResources: TownResourcesService) { }

  resources: ResourceQuantityDto[]|null = null;
  resourceSubscription = Subscription.EMPTY;

  town: TownDto|null = null;
  townLevel = 0;
  emptySpaces: EmptySpace[]|null = null;

  townLoadTimestamp = 0;

  ngOnInit(): void {
    this.loadTown();

    this.resourceSubscription = this.townResources.resources.subscribe({
      next: r => {
        this.resources = r;
      }
    });
  }

  ngOnDestroy() {
    this.clockSubscription.unsubscribe();
  }

  goodiePositions = [
    { left: 75, top: 42 },
    { left: 19, top: 52 },
    { left: 52, top: 90 },
    { left: 65, top: 32 },
    { left: 23, top: 79 },
    { left: 40, top: 40 },
    { left: 12, top: 46 },
    { left: 53, top: 20 },
    { left: 60, top: 10 },
    { left: 63, top: 50 },
    { left: 60, top: 58 },
    { left: 30, top: 80 },
    { left: 26, top: 20 },
    { left: 6, top: 50 },
    { left: 26, top: 87 },
    { left: 38, top: 54 },
    { left: 35, top: 58 },
    { left: 7, top: 35 },
  ];

  buildingYieldProgress: { progress: number, target: number }[] = [];
  buildingPowerAvailable: boolean[] = [];

  private updateBuildingProgress()
  {
    const elapsedSeconds = (Date.now() - this.townLoadTimestamp) / 1000;

    this.buildingYieldProgress = this.town!.buildings.map(b => {
      return {
        progress: b.yieldProgress + elapsedSeconds,
        target: b.secondsRequired
      }
    });

    this.updateBuildingPowersAvailable();

    if(this.buildingYieldProgress.some(b => b.progress >= b.target))
      this.loadTown();
  }

  private updateBuildingPowersAvailable()
  {
    this.buildingPowerAvailable = this.town!.buildings.map(b =>
      !!b.powersAvailableOn && Date.parse(b.powersAvailableOn) < Date.now()
    );
  }

  private loadTown()
  {
    this.api.get<TownDto>('towns/my').subscribe({
      next: r => {
        this.townResources.update(r.data.resources);

        this.town = {
          ...r.data,
          buildings: r.data.buildings.map(b => {
            return {
              ...b,
              ...TownPositions[b.position],
            };
          })
        };

        this.townLevel = Math.min(2, this.town.level);
        this.townLoadTimestamp = Date.now();

        this.updateBuildingProgress();

        this.clockSubscription.unsubscribe();
        this.clockSubscription = interval(HomeComponent.secondsBetweenTick * 1000).subscribe({
          next: () => { this.updateBuildingProgress(); }
        });

        this.emptySpaces = TownPositions
          .map((p, i) => {
            return {
              position: i,
              left: p.left,
              top: p.top,
              minLevel: p.minLevel
            }
          })
          .filter((p, i) =>
            this.town!.level >= p.minLevel &&
            !this.town!.buildings.some(b => b.position == i)
          )
        ;
      }
    });
  }

  doBuildSomething(space: EmptySpace)
  {
    BuildDialog.open(this.matDialog, space.position, this.town!.resources)
      .afterClosed()
      .subscribe({
        next: reload => {
          if(reload)
            this.loadTown();
        }
      })
    ;
  }

  doHarvestBuilding(building: BuildingDto)
  {
    if(building.yield.length == 0)
    {
      if(building.availableSpecializations.length > 0)
      {
        SpecializeDialog.open(this.matDialog, building).afterClosed().subscribe({
          next: newType => {
            if(newType)
              this.loadTown();
          }
        });
      }
      else
      {
        UpgradeDialog.open(this.matDialog, building).afterClosed().subscribe({
          next: upgraded => {
            if(upgraded)
              this.loadTown();
          }
        });
      }
    }
    else
    {
      this.api.post<HarvestDto>('buildings/' + building.id + '/harvest').subscribe({
        next: r => {
          building.yield = [];
          this.mergeResources(r.data.resources);
        },
        error: () => {
          this.loadTown();
        }
      });
    }
  }

  private mergeResources(resources: {type: string, newQuantity: number}[])
  {
    const newResources = this.resources!.map(r => {
      const newResource = resources.find(r2 => r2.type == r.type);
      if(newResource)
        return {
          ...r,
          quantity: newResource.newQuantity
        };
      else
        return r;
    });

    this.townResources.update(newResources);
  }

  doHearRumor()
  {
    if(!this.town?.rumorWaiting)
      return;

    this.town.rumorWaiting = false;

    this.api.post<RumorResponse>('towns/rumor').subscribe({
      next: r => {
        const rumorsSeen = parseInt(window.localStorage.getItem('rumorsSeen') || '0') || 0;

        if(rumorsSeen >= 3)
        {
          OkDialog.open(this.matDialog, 'Rumor', r.data.message);
          window.localStorage.setItem('rumorsSeen', '3+');
        }
        else
        {
          OkDialog.open(this.matDialog, 'Rumor', r.data.message + "\n\n(Check the Mission screen to investigate!)");
          window.localStorage.setItem('rumorsSeen', (rumorsSeen + 1).toString());
        }
      },
      error: () => {
        this.town!.rumorWaiting = true;
      }
    });
  }

  doCollectGoodie(goodie: GoodieDto)
  {
    if(!this.town)
      return;

    this.town.goodies = this.town.goodies.filter(g => g.position != goodie.position);

    this.api.post('towns/my/goodies/' + goodie.position).subscribe({
      next: _ => {
        this.addResources(goodie.type, goodie.quantity);
      },
      error: () => {
        this.loadTown();
      }
    });
  }

  public doRenameTown()
  {
    if(!this.town)
      return;

    RenameTownDialog.open(this.matDialog, this.town.name).afterClosed().subscribe({
      next: r => {
        if(!r || !this.town)
          return;

        this.town.name = r;
      }
    })
  }

  private addResources(type: string, amount: number)
  {
    this.townResources.earn([ { type: type, quantity: amount }]);
  }

  public doUsePower(building: BuildingDto)
  {
    if(!building.powersAvailable)
      return;

    UseBuildingPowerDialog.open(this.matDialog, building).afterClosed().subscribe({
      next: r => {
        if(!r || !this.town) return;

        this.town.buildings = this.town.buildings.map(b => {
          if(b.id != building.id)
            return b;

          return {
            ...b,
            powersAvailableOn: r.powersAvailableOn,
          }
        });

        this.updateBuildingPowersAvailable();
      }
    });
  }

}

interface HarvestDto
{
  resources: {type: string, newQuantity: number}[];
}

interface EmptySpace
{
  position: number;
  left: number;
  top: number;
}

interface RumorResponse
{
  message: string;
}
