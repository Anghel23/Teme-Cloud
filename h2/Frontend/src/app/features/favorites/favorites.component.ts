import { Component, OnInit } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { CommonModule } from '@angular/common';
import { forkJoin } from 'rxjs';

@Component({
  selector: 'app-favorites',
  templateUrl: './favorites.component.html',
  styleUrls: ['./favorites.component.css'],
  imports: [CommonModule],
  standalone: true
})
export class FavoritesComponent implements OnInit {
  backgroundUrl = '/Images/search-background.png';
  favoriteGames: any[] = [];
  userId: string = '';

  constructor(private http: HttpClient) {}

  ngOnInit(): void {
    this.userId = localStorage.getItem('userId') || '';
    this.fetchFavorites();
  }

  fetchFavorites(): void {
    const url = `http://localhost:5103/api/favorites?userId=${this.userId}`;
    const token = localStorage.getItem('token');
  
    const headers = {
      Authorization: `Bearer ${token}`
    };
  
    this.http.get<any[]>(url, { headers }).subscribe({
      next: (favorites) => {
        const rawgIds = favorites.map(fav => fav.rawgId);
        this.fetchGameDetails(rawgIds);
      },
      error: (err) => {
        console.error('Failed to fetch favorites', err);
      }
    });
  }

  fetchGameDetails(rawgIds: string[]): void {
    const requests = rawgIds.map(id =>
      this.http.get<any>(`http://localhost:5103/api/rawg/games/${id}`)
    );
  
    forkJoin(requests).subscribe({
      next: (responses) => {
        console.log('Favorite games:', responses);
        this.favoriteGames = responses.map((game, index) => ({
          ...game,
          rawgId: rawgIds[index]
        }));
      },
      error: (err) => {
        console.error('Failed to fetch game details', err);
      }
    });
  }

  removeFromFavorites(rawgId: string): void {
    if (!rawgId) {
      console.error('Invalid rawgId:', rawgId);
      return;
    }
  
    const url = `http://localhost:5103/api/favorites?userId=${this.userId}&rawgId=${rawgId}`;
    const token = localStorage.getItem('token');
    const headers = { Authorization: `Bearer ${token}` };
  
    this.http.delete(url, { headers }).subscribe({
      next: () => {
        console.log(`Game with rawgId ${rawgId} removed from favorites`);
        this.favoriteGames = this.favoriteGames.filter(game => game.rawgId !== rawgId);
      },
      error: (err) => {
        console.error('Failed to remove game from favorites', err);
      }
    });
  }
}