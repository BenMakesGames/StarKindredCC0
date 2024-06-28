import {Component, EventEmitter, Input, OnChanges, Output} from '@angular/core';
import {ApiService} from "../../../base/services/api.service";

@Component({
  selector: 'app-decoration-box',
  templateUrl: './decoration-box.component.html',
  styleUrls: ['./decoration-box.component.scss']
})
export class DecorationBoxComponent implements OnChanges {

  open = false;
  loaded = false;
  decorations: DecorationDto[] = [];

  @Input() placed!: { type: string }[];
  @Input() maxDecorations: number = 20;

  @Output() decorate = new EventEmitter<{ type: string, mouseX: number, mouseY: number }>();

  placedByType: { [key: string]: number } = {};

  constructor(private api: ApiService) {
  }

  ngOnChanges() {
    this.placedByType = {};

    for(let p of this.placed)
      this.placedByType[p.type] = (this.placedByType[p.type] ?? 0) + 1;
  }

  doToggleNav()
  {
    this.open = !this.open;

    if(!this.loaded)
      this.loadDecorations();
  }

  private loadDecorations()
  {
    this.loaded = true;

    this.api.get<{ decorations: DecorationDto[] }>('treasures/my/decorations').subscribe({
      next: r => {
        this.decorations = r.data.decorations;
      }
    });
  }

  doPlaceDecoration(decoration: DecorationDto, event: MouseEvent)
  {
    if(this.placed.length >= this.maxDecorations)
      return;

    this.decorate.emit({ type: decoration.type, mouseX: event.x, mouseY: event.y });
    this.open = false;
  }
}

interface DecorationDto
{
  type: string;
  quantity: number;
}

