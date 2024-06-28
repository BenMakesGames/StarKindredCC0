import { Component, EventEmitter, Input, Output } from '@angular/core';

@Component({
  selector: 'app-yes-no-toggle',
  templateUrl: './yes-no-toggle.component.html',
  styleUrls: ['./yes-no-toggle.component.scss']
})
export class YesNoToggleComponent {

  @Input() label!: string;
  @Input() value!: boolean;
  @Output() valueChanged = new EventEmitter<boolean>();

  doToggle()
  {
    this.value = !this.value;
    this.valueChanged.emit(this.value);
  }
}
