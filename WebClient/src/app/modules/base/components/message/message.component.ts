import {Component, EventEmitter, Input, OnInit, Output} from '@angular/core';
import {MessageModel} from "../../models/message.model";

@Component({
  selector: 'app-message',
  templateUrl: './message.component.html',
  styleUrls: ['./message.component.scss']
})
export class MessageComponent implements OnInit {

  @Input() message!: MessageModel;
  @Output() click = new EventEmitter();

  constructor() { }

  ngOnInit(): void {
  }

  doClick()
  {
    this.click.emit();
  }

}
