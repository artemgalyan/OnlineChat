import {Component, HostListener, OnDestroy, OnInit} from '@angular/core';
import {StorageService} from "../../shared/services/storage.service";
import {Router} from "@angular/router";
import {Constants} from "../../constants";
import {ChatroomService} from "../shared/services/chatroom.service";
import { HubService } from '../shared/services/hub.service';

@Component({
  selector: 'app-chat',
  templateUrl: './chat.component.html',
  styleUrls: ['./chat.component.css']
})

export class ChatComponent implements OnInit, OnDestroy {
  private backendSender: string = 'Receive';
  private backendGetter: string = 'Connect';
  private readonly chatId: string;
  public sendMessageError = true;
  public userName: string = '';

  public messages: Message[] = [];

  constructor(private storage: StorageService,
              private router: Router,
              private chatService: ChatroomService,
              private hubService: HubService) {
    this.chatId = this.storage.get<string>('chatId');
    this.userName = this.storage.get<string>(Constants.NicknameStorageField);
    this.hubService.addOnHandler(this.backendSender, message => {
      this.messages.push(message);
    });
  }

  ngOnDestroy(): void {
    this.hubService.breakConnection(this.backendSender);
  }

  ngOnInit(): void {
    if(this.chatId == undefined){
      this.router.navigate(['../chats']);
      return;
    }
    this.chatService.getMessages(this.chatId).subscribe(result => {
      this.messages = result.messages;
    });

    this.hubService.invokeBackendMethod(this.backendGetter, this.chatId).catch(
      (reason: string) => {
        alert(reason);
        this.router.navigate(["../chats"])
      });
  }

  async onSendClicked(): Promise<void> {
    const input = document.getElementById('message-input') as HTMLInputElement;
    const message = input.value;
    if (message.length == 0) {
      return;
    }
    input.value = '';
    let value: number = await this.hubService.hubConnection.invoke('Send', this.chatId, message)
      .then()
      .catch(() => alert('Failed to send message'));

    this.sendMessageError = value != 0;
  }

  @HostListener('window:popstate', ['$event'])
  onPopState() {
    this.hubService.breakConnection(this.backendSender);
  }

  async onLeaveClicked() {
    this.hubService.breakConnection(this.backendSender);
    this.router.navigate(['../chats']);
  }
  // private static showErrorMessage(message: string) {
  //   const text = document.getElementById('error-text') as HTMLParagraphElement;
  //   text.textContent = message;
  //   text.hidden = false;
  // }

  // private static hideErrorMessage() {
  //   const text = document.getElementById('error-text') as HTMLParagraphElement;
  //   text.hidden = true;
  // }
  // private static switchResponseCode(code: ConnectionResponseCode) {
  //   switch (code) {
  //     case ConnectionResponseCode.AccessDenied: alert('Access denied'); break;
  //     case ConnectionResponseCode.DuplicateNickname: alert('Duplicate nickname'); break;
  //     case ConnectionResponseCode.BannedNickname: alert('The nickname is banned'); break;
  //     case ConnectionResponseCode.RoomIsFull: alert('Room is full'); break;
  //     case ConnectionResponseCode.WrongNickname: alert('Incorrect nickname'); break;
  //     case ConnectionResponseCode.RoomDoesntExist: alert("Room doesn't exist"); break;
  //   }
  // }
}

export class Message {
  public sender: String = "";
  public text: String = "";
  public sendingTime: Date = new Date(Date.now());
  public status: MessageStatus = MessageStatus.Sent;
}

export enum MessageStatus {
  Sent,
  Pending
}

enum ConnectionResponseCode {
  SuccessfullyConnected = 0,
  AccessDenied,
  DuplicateNickname,
  BannedNickname,
  RoomIsFull,
  WrongNickname,
  RoomDoesntExist
}
