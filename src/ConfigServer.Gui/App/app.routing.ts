import { ModuleWithProviders } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { HomeComponent } from './components/home';
import { ClientComponent } from './components/client';


const appRoutes: Routes = [
    { path: '', component: HomeComponent },
    { path: 'client/:clientId', component: ClientComponent }
];

export const appRoutingProviders: any[] = [

];

export const routing: ModuleWithProviders = RouterModule.forRoot(appRoutes);