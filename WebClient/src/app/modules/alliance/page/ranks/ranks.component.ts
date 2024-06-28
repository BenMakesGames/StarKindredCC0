import {Component, OnDestroy, OnInit} from '@angular/core';
import {ApiService} from "../../../base/services/api.service";
import {catchError, map, of, Subscription} from "rxjs";
import {CreateRankDialog} from "../../dialogs/create-rank/create-rank.dialog";
import {MatDialog} from "@angular/material/dialog";
import {RenameRankDialog} from "../../dialogs/rename-rank/rename-rank.dialog";
import {YesOrNoDialog} from "../../../base/dialogs/yes-or-no/yes-or-no.dialog";

@Component({
  selector: 'app-ranks',
  templateUrl: './ranks.component.html',
  styleUrls: ['./ranks.component.scss']
})
export class RanksComponent implements OnInit, OnDestroy {

  loadSubscription = Subscription.EMPTY;
  titles: TitleDto[]|null = null;

  canEditRanks = false;

  constructor(private api: ApiService, private matDialog: MatDialog) { }

  ngOnInit(): void {
    this.loadSubscription = this.api.get<{ titles: TitleDto[], canEdit: boolean }>('alliances/titles').subscribe({
      next: r => {
        this.titles = r.data.titles;

        this.sortTitles();

        this.canEditRanks = r.data.canEdit;
      }
    })
  }

  ngOnDestroy() {
    this.loadSubscription.unsubscribe();
  }

  doAddRank()
  {
    CreateRankDialog.open(this.matDialog).afterClosed().subscribe({
      next: newTitle => {
        if(!newTitle || !this.titles)
          return;

        this.titles.push(newTitle);

        this.sortTitles();
      }
    });
  }

  private sortTitles()
  {
    if(this.titles)
      this.titles.sort((a, b) => b.rank - a.rank);
  }

  doDeleteTitle(title: TitleDto)
  {
    YesOrNoDialog.open(this.matDialog, () => this.reallyDeleteTitle(title), 'Delete "' + title.title + '"', "Any members with this Title will be demoted to \"No Title\".").afterClosed().subscribe({
      next: _ => {
        if(!this.titles)
          return;

        this.titles = this.titles.filter(t => t.id !== title.id);
      }
    });
  }

  private reallyDeleteTitle(title: TitleDto)
  {
    return this.api.post('alliances/titles/' + title.id + '/delete')
      .pipe(
        map(() => true),
        catchError(() => of(false))
      )
    ;
  }

  doRenameTitle(title: TitleDto)
  {
    RenameRankDialog.open(this.matDialog, title.id, title.title).afterClosed().subscribe({
      next: newTitleName => {
        if(!newTitleName || !this.titles)
          return;

        this.titles = this.titles.map(t => {
          if(t.id == title.id)
            return { ...t, title: newTitleName };
          else
            return t;
        });
      }
    })
  }

}

interface TitleDto
{
  id: string;
  title: string;
  rank: number;
  canRecruit: boolean;
  canKick: boolean;
  canTrackGiants: boolean;
}
