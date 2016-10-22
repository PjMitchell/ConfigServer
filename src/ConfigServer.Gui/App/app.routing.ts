import { ModuleWithProviders } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { HomeComponent } from './components/home';
import { ClientComponent } from './components/client';
import { ClientConfigComponent } from './components/clientConfig';
import { CreateClientComponent } from './components/createClient';

import { EditClientComponent } from './components/editClient';


const appRoutes: Routes = [
    { path: '', component: HomeComponent },
    { path: 'createClient', component: CreateClientComponent },
    { path: 'editClient/:clientId', component: EditClientComponent },
    { path: 'client/:clientId', component: ClientComponent },
    { path: 'client/:clientId/:configurationSetId/:configurationId', component: ClientConfigComponent }
];

export const appRoutingProviders: any[] = [

];

export const routing: ModuleWithProviders = RouterModule.forRoot(appRoutes);