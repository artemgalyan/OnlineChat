import {Injectable} from '@angular/core';
import {HttpClient} from "@angular/common/http";
import {Observable} from "rxjs";
import {ChatroomInfoBase, ChatType, PrivateChatroomInfo} from "../../../shared/chatroom";
import {Constants} from "../../../constants";
import {Message} from 'src/app/chats/chat/chat.component';

@Injectable()
export class ChatroomService {

  private readonly _api = Constants.ApiUrl + '/chatrooms';

  constructor(private httpClient: HttpClient) {
  }

  public getMessages(chatroomId: string): Observable<GetMessagesResponse> {
    return this.httpClient.get<GetMessagesResponse>(this._api + '/get-messages/' + chatroomId, {
      withCredentials: true
    });
  }

  public getChatrooms(): Observable<GetChatroomsResponse> {
    return this.httpClient.get<GetChatroomsResponse>(this._api + '/get-chatrooms', {
      withCredentials: true
    });
  }

  public createChatroom(chatType: ChatType, users: string[], chatName: string): Observable<CreateChatroomResponse> {
    return this.httpClient.post<CreateChatroomResponse>(this._api + '/create',
      {
        'type': chatType,
        'usernames': users,
        'name': chatName
      }, {
        withCredentials: true
      });
  }

  public getChatroomInfo(chatId: string): Observable<ChatroomInfoBase> {
    return this.httpClient.get<ChatroomInfoBase>(
      this._api + '/chat-info/' + chatId,
      {
        withCredentials: true
      }
    )
  }

  filter(chatrooms: PrivateChatroomInfo[], username: string) {
    chatrooms.forEach(c => {
      let index = c.users.indexOf(username, 0);
      if (index > -1) {
        c.users.splice(index, 1);
      }
    });
  }
}

export interface CreateChatroomResponse {
  chatId: string;
  created: boolean;
}

export interface GetMessagesResponse {
  messages: Message[];
}

export interface GetChatroomsResponse {
  chatrooms: ChatroomInfoBase[];
}
