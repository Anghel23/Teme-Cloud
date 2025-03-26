import { Routes } from '@angular/router';
import { HomeComponent } from './features/home/home.component';
import { LoginComponent } from './features/login/login.component';
import { RegisterComponent } from './features/register/register.component';
import { AuthGuard } from './commons/auth/auth.guard';
import { SearchGameComponent } from './features/search-game/search-game.component';
import { GameComponent } from './features/game/game.component';
import { FavoritesComponent } from './features/favorites/favorites.component';

export const routes: Routes = [
    { path: '', redirectTo: 'home', pathMatch: 'full' },
    { path: 'home', component: HomeComponent },
    { path: 'login', component: LoginComponent , canActivate: [AuthGuard]},
    { path: 'register', component: RegisterComponent , canActivate: [AuthGuard]},
    { path: 'search-game', component: SearchGameComponent },
    { path: 'game/:gameName', component: GameComponent },
    { path: 'favorites', component: FavoritesComponent },
    { path: '**', redirectTo: 'home' }
];
