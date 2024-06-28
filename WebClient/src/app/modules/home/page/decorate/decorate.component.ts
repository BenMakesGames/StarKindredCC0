import {Component, ElementRef, HostListener, OnInit, ViewChild} from '@angular/core';
import {ApiService} from "../../../base/services/api.service";
import {TownResourcesService} from "../../../base/services/town-resources.service";
import {TownPositions} from "../../helpers/town-positions";
import {Router} from "@angular/router";
import {DecorationDto, TownDto} from "../../../../dtos/town.dto";
import {YesOrNoDialog} from "../../../base/dialogs/yes-or-no/yes-or-no.dialog";
import {MatDialog} from "@angular/material/dialog";
import {Observable, Subject} from "rxjs";
import {MessagesService} from "../../../base/services/messages.service";
import {NavMenuService} from "../../../base/services/nav-menu.service";

@Component({
  selector: 'app-decorate',
  templateUrl: './decorate.component.html',
  styleUrls: ['./decorate.component.scss']
})
export class DecorateComponent implements OnInit {

  @ViewChild('main') main!: ElementRef;

  town: TownDto|null = null;
  hasChanges = false;
  saving = false;

  dragging: DecorationDto|null = null;
  dragOrigin: any|null = null;
  decorations: DecorationDto[] = [];
  showMoreMenu = false;
  previewMode = false;

  useAdvancedTools = false;
  advancedEditorLeft = false;

  @ViewChild('moreMenu') moreMenu!: ElementRef;

  @HostListener('document:click', ['$event'])
  doDocumentClick(event: any) {
    this.showMoreMenu = false;
    this.setPreviewMode(false);
  }

  doToggleMoreMenu(event: any)
  {
    this.showMoreMenu = !this.showMoreMenu;
    event.stopPropagation();
  }

  constructor(
    private api: ApiService, private townResources: TownResourcesService, private router: Router,
    private matDialog: MatDialog, private messages: MessagesService, private navMenuService: NavMenuService
  )
  {
  }

  ngOnInit(): void {
    this.navMenuService.hide();
    this.loadTown();

    if(window.localStorage.getItem('useAdvancedTools') === 'yeah') {
      this.useAdvancedTools = true;
    }

    if(window.localStorage.getItem('advancedEditorPlacement') === 'left') {
      this.advancedEditorLeft = true;
    }
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

        this.decorations = this.town.decorations;
      }
    });
  }

  doRemove()
  {
    if(!this.dragging) return;

    this.decorations = this.decorations.filter(d => d != this.dragging);
    this.clearSelection();
  }

  doFlip()
  {
    if(!this.dragging) return;

    this.dragging.flipX = !this.dragging.flipX;
    this.hasChanges = true;
  }

  doExpand()
  {
    if(!this.dragging) return;

    this.dragging.scale = Math.min(200, this.dragging.scale + 10);
    this.hasChanges = true;
  }

  doShrink()
  {
    if(!this.dragging) return;

    this.dragging.scale = Math.max(50, this.dragging.scale - 10);
    this.hasChanges = true;
  }

  doClickMap(event: MouseEvent)
  {
    if(this.previewMode) return;

    if(this.dragging)
    {
      const eventPos = this.screenToPosition(
        event.x + this.main.nativeElement.scrollLeft - this.dragOrigin.scrollX,
        event.y + this.main.nativeElement.scrollTop - this.dragOrigin.scrollY
      );

      const originPos = this.screenToPosition(this.dragOrigin.mouseX, this.dragOrigin.mouseY);

      this.dragging.x = this.dragOrigin.decorationX + eventPos.x - originPos.x;
      this.dragging.y = this.dragOrigin.decorationY + eventPos.y - originPos.y;

      this.decorations = this.decorations.map(d => {
        if(d != this.dragging)
          return d;

        return this.dragging;
      });

      if(!this.useAdvancedTools)
        this.clearSelection();
    }
  }

  public doMoveAdvancedEditor()
  {
    this.advancedEditorLeft = !this.advancedEditorLeft;

    window.localStorage.setItem('advancedEditorPlacement', this.advancedEditorLeft ? 'left' : 'right');
  }

  public clearSelection()
  {
    this.dragging = null;
    this.dragOrigin = null;
    this.hasChanges = true;
  }

  doClickDecoration(event: MouseEvent, d: DecorationDto)
  {
    if(this.previewMode) return;

    if(this.dragging == null)
    {
      this.dragging = d;
      this.dragOrigin = {
        decorationX: this.dragging.x,
        decorationY: this.dragging.y,
        mouseX: event.x,
        mouseY: event.y,
        scrollX: this.main.nativeElement.scrollLeft,
        scrollY: this.main.nativeElement.scrollTop
      };

      this.showMoreMenu = false;
      event.stopPropagation();
    }
  }

  positionToScreen(x: number, y: number)
  {
    const scale = Math.max(Math.min(960, window.innerWidth), window.innerHeight);

    return {
      x: scale * x / 100,
      y: scale * y / 100
    };
  }

  screenToPosition(x: number, y: number)
  {
    const scale = Math.max(Math.min(960, window.innerWidth), window.innerHeight);

    return {
      x: x * 100 / scale,
      y: y * 100 / scale
    };
  }

  doDecorate(e: { type: string, mouseX: number, mouseY: number })
  {
    const decorationPos = this.screenToPosition(
      e.mouseX - this.main.nativeElement.offsetLeft + this.main.nativeElement.scrollLeft,
      e.mouseY + this.main.nativeElement.scrollTop
    );

    this.decorations.push({
      type: e.type,
      x: decorationPos.x - 3.5,
      y: decorationPos.y - 7,
      scale: 100,
      flipX: false,
    });

    this.dragging = this.decorations[this.decorations.length - 1];

    this.dragOrigin = {
      decorationX: this.dragging.x,
      decorationY: this.dragging.y,
      mouseX: e.mouseX,
      mouseY: e.mouseY,
      scrollX: this.main.nativeElement.scrollLeft,
      scrollY: this.main.nativeElement.scrollTop
    };
  }

  doStopEditing()
  {
    if(!this.hasChanges)
    {
      // noinspection JSIgnoredPromiseFromCall
      this.router.navigateByUrl('/home');
      return;
    }

    this.saving = true;

    this.api.post('towns/my/decorations', { decorations: this.decorations }).subscribe({
      next: () => {
        // noinspection JSIgnoredPromiseFromCall
        this.router.navigateByUrl('/home');
      },
      error: () => {
        this.saving = false;
      }
    });
  }

  doClearAllDecorations()
  {
    YesOrNoDialog.open(this.matDialog, () => this.reallyClearAllDecorations(), 'Clear All Decorations?');
  }

  private reallyClearAllDecorations(): Observable<boolean>
  {
    const result = new Subject<boolean>();

    this.api.post('towns/my/decorations/clearAll').subscribe({
      next: _ => {
        this.decorations = [];
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

  doShowAdvancedTools()
  {
    this.useAdvancedTools = !this.useAdvancedTools;

    window.localStorage.setItem('useAdvancedTools', this.useAdvancedTools ? 'yeah' : 'nah');
  }

  private setPreviewMode(previewMode: boolean)
  {
    if(this.previewMode == previewMode) return;

    this.previewMode = previewMode;
  }

  doPreviewMode(event: any)
  {
    event.stopPropagation();
    this.showMoreMenu = false;
    this.setPreviewMode(true);
    this.clearSelection();

    const clickOrTouch = window.navigator.maxTouchPoints > 1 ? 'Touch' : 'Click';

    this.messages.add({
      type: 'Info',
      text: clickOrTouch + ' anywhere to exit preview mode.',
    });
  }
}
