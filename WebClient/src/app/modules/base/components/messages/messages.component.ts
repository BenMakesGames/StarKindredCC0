import { Component, OnInit } from '@angular/core';
import {MessagesService} from "../../services/messages.service";
import {MessageModel} from "../../models/message.model";

@Component({
  selector: 'app-messages',
  templateUrl: './messages.component.html',
  styleUrls: ['./messages.component.scss']
})
export class MessagesComponent implements OnInit {

  nextId = 0;

  constructor(private messagesService: MessagesService) { }

  messages: VisibleMessage[] = [];

  ngOnInit(): void {
    this.messagesService.messages.subscribe({
      next: m => {
        const id = this.nextId;
        this.nextId++;

        const timeout = setTimeout(() => {
          this.doRemoveMessage(id);
        }, 5000);

        this.messages.push({
          id: id,
          message: m,
          subscription: timeout
        });
      }
    });
  }

  doRemoveMessage(id: number)
  {
    const messageIndex = this.messages.findIndex(m => m.id === id)
    const message = this.messages[messageIndex];

    if(!message)
      return;

    clearTimeout(message.subscription);
    this.messages.splice(messageIndex, 1);
  }
}

interface VisibleMessage
{
  id: number;
  subscription: any;
  message: MessageModel;
}
