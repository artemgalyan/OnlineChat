import {NgModule} from '@angular/core';
import {BrowserModule} from '@angular/platform-browser';

import {AppRoutingModule} from './app-routing.module';
import {AppComponent} from './app.component';
import {HttpClientModule} from "@angular/common/http";
import {FormsModule} from "@angular/forms";
import {AuthModule} from './auth/auth.module';
import {ChatsModule} from './chats/chats.module';
import {ChatsGuard} from './shared/guards/chats-guard.service';
import {AuthGuard} from './shared/guards/auth-guard.service';
import {StorageService} from './shared/services/storage.service';
import {AuthenticationService} from './shared/services/authentication.service';
import {ConversationsComponent} from './conversations/conversations.component';

@NgModule({
  declarations: [
    AppComponent,
    ConversationsComponent
  ],
  imports: [
    BrowserModule,
    AppRoutingModule,
    HttpClientModule,
    FormsModule,
    AuthModule,
    ChatsModule
  ],
  providers: [HttpClientModule, ChatsGuard, AuthGuard, StorageService, AuthenticationService],
  bootstrap: [AppComponent]
})

export class AppModule {
}
