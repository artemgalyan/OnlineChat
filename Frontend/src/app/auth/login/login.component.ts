import { Component, OnInit } from '@angular/core';
import {StorageService} from "../../shared/services/storage.service";
import {Router} from "@angular/router";
import {Constants} from "../../constants";
import {AuthenticationService, LoginResponse, TokenLoginResponseCode} from "../../shared/services/authentication.service";

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.css']
})
export class LoginComponent implements OnInit {

  public username: string = "";
  public password: string = "";
  public errorMessage: string = "";

  constructor(private storage: StorageService,
              private router: Router,
              private authService: AuthenticationService) {

  }

  ngOnInit(): void {
  }

  async onConnectClicked(): Promise<void> {
    if (this.username == null || this.password == null) {
      this.showErrorMessage('Password or nickname is null.');
      return;
    }
    if (!this.checkNickname(this.username)) {
      this.showErrorMessage('Invalid nickname');
      return;
    }
    this.authService.login(this.username, this.password)
      .subscribe(response => {
      if (response == LoginResponse.Success) {
        this.onLoggedIn(this.username);
        return;
      }
      if (response == LoginResponse.WrongPassword) {
        this.showErrorMessage('Wrong password');
        return;
      }
      this.showErrorMessage('Wrong username');
    });
  }

  checkNickname(nickname: string): boolean {
    return nickname !== '' && nickname !== 'admin';
  }

  showErrorMessage(message: string) : void {
    this.errorMessage = message;
  }

  private onLoggedIn(username: string) {
    this.storage.setLoggedIn(true);
    this.storage.set(Constants.NicknameStorageField, username);
    this.router.navigate(['/conversations']);
  }
}
