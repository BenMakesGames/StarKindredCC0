import { Injectable } from '@angular/core';
import { Subject} from "rxjs";
import {MessageModel} from "../models/message.model";

@Injectable({
  providedIn: 'root'
})
export class MessagesService {

  messages = new Subject<MessageModel>();

  constructor() { }

  add(message: MessageModel)
  {
    this.messages.next(message);
  }
}
