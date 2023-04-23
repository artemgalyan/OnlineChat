import {Message} from "../chats/chat/chat.component";

export enum ChatType {
  Public = 0,
  Private
}

export interface ChatroomInfoBase {
  id: string;
  users: string[];
  type: ChatType;
  lastMessage: Message;
  unreadMessages: number;
}

export interface PrivateChatroomInfo extends ChatroomInfoBase {}

export interface PublicChatroomInfo extends ChatroomInfoBase {
  name: string;
  moderators: string[];
}
