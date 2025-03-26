import { Component } from '@angular/core';
import { HttpClient, HttpClientModule, HttpParams } from '@angular/common/http';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';

@Component({
  selector: 'app-search-game',
  templateUrl: './search-game.component.html',
  styleUrls: ['./search-game.component.css'],
  imports: [CommonModule, FormsModule, HttpClientModule],
  standalone: true
})
export class SearchGameComponent {
  backgroundUrl = '/Images/search-background.png';

  searchTerm: string = '';
  games: any[] = [];
  backgroundDefault = '/Images/search-game-background.png';

  constructor(private http: HttpClient, private router: Router) {}

  searchGames() {
    const params = new HttpParams()
      .set('search', this.searchTerm)
      .set('page', 1)
      .set('pageSize', 10);

    this.http.get<any>('http://localhost:5103/api/rawg/games', { params }).subscribe({
      next: (response) => {
        const parsed = typeof response === 'string' ? JSON.parse(response) : response;
        this.games = parsed.results || [];
      },
      error: (err) => {
        console.error('Search failed', err);
      }
    });
  }

  navigateToGame(gameName: string): void {
    this.router.navigate(['/game', gameName]);
  }
}
