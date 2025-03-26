import { Component } from '@angular/core';
import { Router } from '@angular/router';
import { PlatformService } from '../../commons/platform/platform.service';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';

@Component({
  selector: 'app-navbar',
  templateUrl: './navbar.component.html',
  styleUrl: './navbar.component.css',
  standalone: true,
  imports: [CommonModule, RouterModule]
})
export class NavbarComponent {
  constructor(private router: Router, private platformService: PlatformService) {}

  isLoggedIn(): boolean {
    if (this.platformService.isBrowser()) {
      return !!localStorage.getItem('token');
    }
    return false;
  }

  logout(): void {
    if (this.platformService.isBrowser()) {
      localStorage.removeItem('token');
    }
    this.router.navigate(['/home']);
  }
}