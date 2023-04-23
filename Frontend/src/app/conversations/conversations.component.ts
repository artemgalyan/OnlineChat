import {Component, OnInit} from '@angular/core';
import {ChatroomInfoBase, ChatType, PrivateChatroomInfo, PublicChatroomInfo} from "../shared/chatroom";
import {StorageService} from "../shared/services/storage.service";
import {Router} from "@angular/router";
import {ChatroomService} from "../chats/shared/services/chatroom.service";
import {AuthenticationService} from "../shared/services/authentication.service";
import {HubService} from "../chats/shared/services/hub.service";
import {Constants} from "../constants";
import {Message} from "../chats/chat/chat.component";

@Component({
  selector: 'app-conversations',
  templateUrl: './conversations.component.html',
  styleUrls: ['./conversations.component.css']
})
export class ConversationsComponent implements OnInit {
  public chatrooms: ChatroomInfoBase[] = [];
  public messages: Message[] = []
  public isAnyChatSelected = false
  public message: string = ""
  private chatId: string = ""
  private readonly maxTextLength: number = 50

  constructor(private storage: StorageService,
              public router: Router,
              private chatroomsService: ChatroomService,
              public auth: AuthenticationService,
              private hubService: HubService) {
  }

  ngOnInit(): void {
    this.hubService.addOnHandler('Receive', message => {
      this.messages.push(message as Message);
    });
    this.hubService.hubConnection.on('AddChatroom', c => {
      this.chatrooms.unshift(c)
      // console.log(c);
    });
    this.updateChatrooms().then(
      () => {
        this.hubService.addOnHandler('PromoteToTop', (chatId: string, m: Message) => this.handlePromotion(chatId, m));
      },
      (reason) => {
        alert(reason)
      })
  }

  private handlePromotion(toPromoteChatId: string, m: Message) {
    let filtered = this.chatrooms.filter(s => s.id == toPromoteChatId);
    if (filtered.length === 0) {
      this.chatroomsService.getChatroomInfo(toPromoteChatId)
        .subscribe(result => {
          this.chatrooms.unshift(result)
          this.scrollToEnd()
        })
    } else {
      let chatroom = filtered[0];
      chatroom.lastMessage = m;
      if (toPromoteChatId != this.chatId) {
        chatroom.unreadMessages++
      }
      let index = this.chatrooms.indexOf(chatroom);
      this.chatrooms.splice(index, 1);
      this.chatrooms.unshift(chatroom)
      this.scrollToEnd()
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
    if (room.type == ChatType.Public) {
      return (room as PublicChatroomInfo).name;
    }
    return 'Private chat with ' + this.getSecondUserForPrivateChat(room as PrivateChatroomInfo) ;
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

  getSendingTime(d: Date): string {
    if (d === null) {
      return ""
    }
    let date = new Date(d)
    let mins = date.getMinutes()
    return date.getHours() + ':' + (mins < 10 ? '0' : '') + date.getMinutes();
  }

  getLastMessage(room: ChatroomInfoBase) : string {
    if (room === null || room.lastMessage === null) {
      return "No messages"
    }
    let text = room.lastMessage.text
    if (text.length > this.maxTextLength) {
      text = text.slice(0, this.maxTextLength - 3) + '...'
    }
    return room.lastMessage.sender + ': ' + text
  }

  setActiveChat(chatId: string) {
    if (chatId == this.chatId) {
      this.removeSelection(document.getElementById(chatId)!)
      this.chatId = "";
      this.isAnyChatSelected = false;
      this.messages = [];
      return;
    }
    let oldChat = document.getElementById(this.chatId);
    if (oldChat !== null) {
      this.removeSelection(oldChat)
    }
    this.hubService.hubConnection.invoke('Disconnect', this.chatId)
    let chatrooms = this.chatrooms.filter(c => c.id == chatId);
    if (chatrooms.length === 0) {
      return
    }

    this.chatroomsService.getMessages(chatId).subscribe(result => {
      this.hubService.invokeBackendMethod('Connect', chatId).then(() => this.scrollToEnd())
      this.messages = result.messages;
      this.isAnyChatSelected = true;
      this.chatId = chatId
      chatrooms[0].unreadMessages = 0;
      let element = document.getElementById(chatId)
      this.addSelection(element!)
    });
  }

  private findChatById(chatId: string) : ChatroomInfoBase | any {
    let chatrooms = this.chatrooms.filter(c => c.id == chatId);
    if (chatrooms.length === 0) {
      return null
    }
    return chatrooms[0]
}

  sendMessage() {
    this.hubService.hubConnection.invoke('Send', this.chatId, this.message).then(() => {
      this.message = "";
      this.scrollToEnd()
    })
  }

  scrollToEnd() {
    let element = document.getElementsByClassName('messages')[0];
    element!.scrollTop = element!.scrollHeight
  }

  private addSelection(e: HTMLElement) {
    e.style.backgroundColor = '#8cd7fa';
  }
  private removeSelection(e: HTMLElement) {
    e.style.backgroundColor = 'white'
  }
}
