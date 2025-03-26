import { Injectable } from '@angular/core';
import { CanActivate, Router } from '@angular/router';
import { PlatformService } from '../platform/platform.service';

@Injectable({
  providedIn: 'root'
})
export class AuthGuard implements CanActivate {
  constructor(private router: Router, private platformService: PlatformService) {}

  canActivate(): boolean {
    if (this.platformService.isBrowser() && localStorage.getItem('token')) {
      this.router.navigate(['/home']);
      return false;
    }
    return true;
  }
}