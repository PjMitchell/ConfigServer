import { ModuleWithProviders } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { HomeComponent } from './components/home';
import { ClientOverviewComponent } from './components/clientOverview';
import { ClientConfigShellComponent } from './components/clientConfigShell';
import { CreateClientComponent } from './components/createClient';
import { EditGroupClientsComponent } from './components/editGroupClients';

import { EditClientComponent } from './components/editClient';


const appRoutes: Routes = [
    { path: '', component: HomeComponent },
    { path: 'createClient', component: CreateClientComponent },
    { path: 'editClient/:clientId', component: EditClientComponent },
    { path: 'client/:clientId', component: ClientOverviewComponent },
    { path: 'client/:clientId/:configurationSetId/:configurationId', component: ClientConfigShellComponent },
    { path: 'editGroupClients', component: EditGroupClientsComponent }
];

export const appRoutingProviders: any[] = [

];

export const routing: ModuleWithProviders = RouterModule.forRoot(appRoutes);