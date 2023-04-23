import {Component, OnDestroy, OnInit} from '@angular/core';
import {StorageService} from "../../shared/services/storage.service";
import {Constants} from "../../constants";
import {Router} from "@angular/router";
import {ChatroomService} from "../shared/services/chatroom.service";
import {ChatroomInfoBase, ChatType, PrivateChatroomInfo, PublicChatroomInfo} from "../../shared/chatroom";
import {AuthenticationService} from 'src/app/shared/services/authentication.service';
import {HubService} from '../shared/services/hub.service';

@Component({
  selector: 'app-chat-selector',
  templateUrl: './chat-selector.component.html',
  styleUrls: ['./chat-selector.component.css']
})
export class ChatSelectorComponent implements OnInit, OnDestroy {
  public chatrooms: ChatroomInfoBase[] = [];

  private readonly timesToUpdateChatrooms: number = 3;
  private timesUpdatedChatrooms: number = 0;
  private chatId: string = null!;

  constructor(private storage: StorageService,
              private router: Router,
              private chatroomsService: ChatroomService,
              public auth: AuthenticationService,
              private hubService: HubService) {

  }

  ngOnInit(): void {
    this.updateChatrooms().then(
      () => {
        this.hubService.addOnHandler('PromoteToTop', (chatId: string) => this.handlePromotion(chatId));
      },
      (reason) => {
        alert(reason)
      })
  }

  ngOnDestroy(): void {
    this.hubService.breakConnection('PromoteToTop');
  }

  private handlePromotion(toPromoteChatId: string) {
    let filtered = this.chatrooms.filter(s => s.id == toPromoteChatId);
    if (filtered.length === 0) {
      this.chatroomsService.getChatroomInfo(toPromoteChatId)
        .subscribe(result => this.chatrooms.unshift(result))
    } else {
      let chatroom = filtered[0];
      let index = this.chatrooms.indexOf(chatroom);
      this.chatrooms.slice(index, 1);
    }
  }

  private updateChatrooms(): Promise<void> {
    return new Promise((resolve, reject) => this.chatroomsService.getChatrooms()
      .subscribe(result => {
        if (result == null || result.chatrooms == null) {
          reject('Unknown error');
        }
        this.chatrooms = result!.chatrooms;
        resolve();
      }))
  }

  getRoomName(room: ChatroomInfoBase): string {
    //console.log('hi!' + room);
    if (room.type == ChatType.Public) {
      return (room as PublicChatroomInfo).name + ' ' + room.unreadMessages;
    }
    return 'Private chat with ' + this.getSecondUserForPrivateChat(room as PrivateChatroomInfo) + ' ' + room.unreadMessages;
  }

  public joinChatroom(chatId: string) {
    this.chatId = chatId;
    this.storage.set(Constants.ChatIdStorageField, chatId);
    this.router.navigate(['chats/chat']);
  }

  public createChatroom() {
    this.router.navigate(['chats/create']);
  }

  isPublic(room: ChatroomInfoBase) {
    return room.type == ChatType.Public;
  }


  getSecondUserForPrivateChat(room: PrivateChatroomInfo): string {
    const username = this.storage.get<string>(Constants.NicknameStorageField);
    for (let user of room.users) {
      if (user != username) {
        return user;
      }
    }

    return "";
  }
}
