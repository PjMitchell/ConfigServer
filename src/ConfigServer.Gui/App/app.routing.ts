import { ModuleWithProviders } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { HomeComponent } from './components/home';
import { ClientComponent } from './components/client';
import { ClientConfigComponent } from './components/clientConfig';



const appRoutes: Routes = [
    { path: '', component: HomeComponent },
    { path: 'client/:clientId', component: ClientComponent },
    { path: 'client/:clientId/:configurationSetId/:configurationId', component: ClientConfigComponent }

];

export const appRoutingProviders: any[] = [

];

export const routing: ModuleWithProviders = RouterModule.forRoot(appRoutes);