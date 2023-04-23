import { NgModule } from "@angular/core";
import { RouterModule, Routes } from "@angular/router";
import { ChatsGuard } from "../shared/guards/chats-guard.service";
import { ChatSelectorComponent } from "./chat-selector/chat-selector.component";
import { ChatComponent } from "./chat/chat.component";
import { CreateChatComponent } from "./create-chat/create-chat.component";

const chatRoutes: Routes = [
    {path: '', component: ChatSelectorComponent, canActivate: [ChatsGuard], children: [
        {path: 'create', component: CreateChatComponent, canActivate: [ChatsGuard]},
        {path: 'chat', component: ChatComponent, canActivate: [ChatsGuard]},
    ]},
]

@NgModule({
    imports: [RouterModule.forChild(chatRoutes)],
    exports: [RouterModule]
})

export class ChatsRoutingModule {}
