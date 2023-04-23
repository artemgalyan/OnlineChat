import {NgModule} from '@angular/core';
import {RouterModule, Routes} from '@angular/router';
import { AppComponent } from './app.component';
import { AuthGuard } from './shared/guards/auth-guard.service';
import { ChatsGuard } from './shared/guards/chats-guard.service';
import {ConversationsComponent} from "./conversations/conversations.component";
import {CreateChatComponent} from "./chats/create-chat/create-chat.component";
import {LocationStrategy, PathLocationStrategy} from "@angular/common";

const routes: Routes = [
  {path: '', component: AppComponent},
  {path: 'auth', canActivate: [AuthGuard], loadChildren: () => import('./auth/auth.module').then(m => m.AuthModule)},
  {path: 'chats', canActivate: [ChatsGuard], loadChildren: () => import('./chats/chats.module').then(m => m.ChatsModule)},
  {path: 'conversations', canActivate: [ChatsGuard], component: ConversationsComponent},
  {path: 'create-new', component: CreateChatComponent},
  {path: '**', redirectTo: 'auth/login'}
];

@NgModule({
  imports: [RouterModule.forRoot(routes, {onSameUrlNavigation: 'reload'})],
  exports: [RouterModule]
})
export class AppRoutingModule {
}
