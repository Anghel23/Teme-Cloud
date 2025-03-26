import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { HttpClient, HttpClientModule } from '@angular/common/http';
import { CommonModule } from '@angular/common';
import { SafePipe } from '../../shared/pipe/pipe-yt.pipe';

@Component({
  selector: 'app-game',
  templateUrl: './game.component.html',
  styleUrls: ['./game.component.css'],
  imports: [CommonModule, SafePipe, HttpClientModule],
  standalone: true
})
export class GameComponent implements OnInit {
  gameName: string = '';
  game: any = {};
  isFavorite: boolean = false;
  userId: string = '';
  trailerVideoId: string | null = null;
  trailerTitle: string = '';

  constructor(private route: ActivatedRoute, private http: HttpClient) {}

  ngOnInit(): void {
    this.userId = localStorage.getItem('userId') || '';
    this.gameName = this.route.snapshot.paramMap.get('gameName') || 'Unknown Game';
    this.fetchGameDetails();
    this.checkIfFavorite();
    this.fetchTrailer(this.gameName);
  }

  fetchGameDetails(): void {
    const apiUrl = `http://localhost:5103/api/rawg/games?search=${this.gameName}`;
    this.http.get<any>(apiUrl).subscribe({
      next: (response) => {
        if (response.results && response.results.length > 0) {
          this.game = response.results[0];
          this.setTrailerDetails();
        }
      },
      error: (err) => {
        console.error('Failed to fetch game details', err);
      }
    });
  }

  setTrailerDetails(): void {
    if (this.game.clip && this.game.clip.video) {
      this.trailerVideoId = this.extractYouTubeVideoId(this.game.clip.video);
      this.trailerTitle = `${this.game.name} Trailer`;
    }
  }

  extractYouTubeVideoId(url: string): string | null {
    const match = url.match(/(?:https?:\/\/)?(?:www\.)?youtube\.com\/.*v=([^&]+)/) || url.match(/youtu\.be\/([^?]+)/);
    return match ? match[1] : null;
  }

  checkIfFavorite(): void {
    const url = `http://localhost:5103/api/favorites?userId=${this.userId}`;
    const token = localStorage.getItem('token');
    const headers = { Authorization: `Bearer ${token}` };
  
    this.http.get<any[]>(url, { headers }).subscribe({
      next: (favorites) => {
        this.isFavorite = favorites.some(fav => fav.rawgId === this.game.id);
      },
      error: (err) => {
        console.error('Failed to check favorites', err);
      }
    });
  }

  toggleFavorite(): void {
    if (this.isFavorite) {
      this.removeFromFavorites();
    } else {
      this.addToFavorites();
    }
  }
  
  addToFavorites(): void {
    const url = `http://localhost:5103/api/favorites`;
    const token = localStorage.getItem('token');
    const headers = {
      Authorization: `Bearer ${token}`,
      'Content-Type': 'application/json'
    };
  
    const body = {
      rawgId: this.game.id.toString(),
      userId: this.userId
    };
  
    console.log('Adding to favorites:', body);
  
    this.http.post(url, body, { headers }).subscribe({
      next: () => {
        this.isFavorite = true;
        console.log('Game added to favorites');
      },
      error: (err) => {
        console.error('Failed to add to favorites', err);
        console.log('Error details:', err.error);
      }
    });
  }
  
  removeFromFavorites(): void {
    const url = `http://localhost:5103/api/favorites?userId=${this.userId}&rawgId=${this.game.id}`;
    const token = localStorage.getItem('token');
    const headers = { Authorization: `Bearer ${token}` };
  
    this.http.delete(url, { headers }).subscribe({
      next: () => {
        this.isFavorite = false;
        console.log('Game removed from favorites');
      },
      error: (err) => {
        console.error('Failed to remove from favorites', err);
      }
    });
  }

  fetchTrailer(gameName: string): void {
    const url = `http://localhost:5103/api/youtube/trailer?gameName=${encodeURIComponent(gameName)}`;
    this.http.get<any>(url).subscribe({
      next: (data) => {
        this.trailerVideoId = data.videoId;
        this.trailerTitle = data.title;
      },
      error: (err) => {
        console.error('Failed to fetch trailer', err);
      }
    });
  }

  isLoggedIn(): boolean {
    return !!this.userId && !!localStorage.getItem('token');
  }

}