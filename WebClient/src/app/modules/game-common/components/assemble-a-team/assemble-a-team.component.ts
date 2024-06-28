import {Component, EventEmitter, Input, OnInit, Output} from '@angular/core';
import {VassalSearchResultModel} from "../../../../dtos/vassal-search-result.model";

@Component({
  selector: 'app-assemble-a-team',
  templateUrl: './assemble-a-team.component.html',
  styleUrls: ['./assemble-a-team.component.scss']
})
export class AssembleATeamComponent implements OnInit {

  @Input() embarking = false;
  @Input() minVassals = 1;
  @Input() maxVassals!: number;
  @Input() tags!: { title: string, color: string }[];
  @Input() requiredElement: string|null = null;

  @Output() cancel = new EventEmitter<void>();
  @Output() select = new EventEmitter<void>();

  @Input() selectedVassals: VassalSearchResultModel[] = [];
  @Output() selectedVassalsChange = new EventEmitter<VassalSearchResultModel[]>();

  @Input() extraInfo: ((v: VassalSearchResultModel) => string)|null = null;

  requirementsAreMet = false;

  query = {
    name: null,
    onMission: false,
    isLeader: false,
    page: 1
  };

  constructor() { }

  ngOnInit(): void {
  }

  canAdd = (vassal: VassalSearchResultModel) =>
  {
    if(this.selectedVassals.length >= this.maxVassals)
      return false;

    if(this.selectedVassals.some(s => s.id == vassal.id))
      return false;

    // TODO: check wounds

    return true;
  }

  doRemoveVassal(vassal: VassalSearchResultModel)
  {
    const selectedVassals = this.selectedVassals.filter(v => v.id !== vassal.id);
    this.selectedVassalsChange.emit(selectedVassals);

    this.checkRequirements(selectedVassals);
  }

  doAddVassal(vassal: VassalSearchResultModel)
  {
    if(this.selectedVassals.length >= this.maxVassals)
      return;

    const selectedIndex = this.selectedVassals.findIndex(s => s.id == vassal.id);

    if(selectedIndex >= 0)
      return;

    const selectedVassals = this.selectedVassals.concat(vassal);

    this.checkRequirements(selectedVassals);

    this.selectedVassalsChange.emit(selectedVassals);
  }

  private checkRequirements(selectedVassals: VassalSearchResultModel[])
  {
    this.requirementsAreMet = this.computeRequirementsAreMet(selectedVassals);
  }

  private computeRequirementsAreMet(selectedVassals: VassalSearchResultModel[]): boolean
  {
    if(selectedVassals.length < this.minVassals || selectedVassals.length > this.maxVassals)
      return false;

    if(this.requiredElement)
    {
      if(!selectedVassals.some(v => v.element == this.requiredElement))
        return false;
    }

    return true;
  }
}
