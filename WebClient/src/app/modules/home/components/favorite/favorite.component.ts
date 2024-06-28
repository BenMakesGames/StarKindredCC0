import {Component, EventEmitter, Input, Output} from '@angular/core';
import {ApiService} from "../../../base/services/api.service";

@Component({
  selector: 'app-favorite',
  templateUrl: './favorite.component.html',
  styleUrls: ['./favorite.component.scss']
})
export class FavoriteComponent {

  @Input() favorite!: boolean;
  @Output() favoriteChange = new EventEmitter<boolean>();

  @Input() vassalId: string|null = null!;

  saving = false;

  constructor(private api: ApiService) {
  }

  doFavorite()
  {
    if(this.saving) return;

    this.saving = true;

    if(this.favorite)
    {
      this.api.post('vassals/' + this.vassalId + '/unFavorite').subscribe({
        next: () => {
          this.favoriteChange.emit(false);
          this.saving = false;
        },
        error: () => {
          this.saving = false;
        }
      });
    }
    else
    {
      this.api.post('vassals/' + this.vassalId + '/favorite').subscribe({
        next: () => {
          this.favoriteChange.emit(true);
          this.saving = false;
        },
        error: () => {
          this.saving = false;
        }
      });
    }
  }
}
