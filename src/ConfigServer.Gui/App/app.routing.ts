import { ModuleWithProviders } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { HomeComponent } from './components/home';
import { ClientOverviewComponent } from './components/clientOverview';
import { ClientConfigShellComponent } from './components/clientConfigShell';
import { CreateClientComponent } from './components/createClient';
import { GroupClientsComponent } from './components/groupClients';

import { EditClientComponent } from './components/editClient';
import { CreateClientGroupComponent } from './components/createClientGroup';
import { EditClientGroupComponent } from './components/editClientGroup';



const appRoutes: Routes = [
    { path: '', component: HomeComponent },
    { path: 'createClient', component: CreateClientComponent },
    { path: 'editClient/:clientId', component: EditClientComponent },
    { path: 'createClientGroup', component: CreateClientGroupComponent },
    { path: 'editClientGroup/:groupId', component: EditClientGroupComponent },
    { path: 'client/:clientId', component: ClientOverviewComponent },
    { path: 'client/:clientId/:configurationSetId/:configurationId', component: ClientConfigShellComponent },
    { path: 'group', component: GroupClientsComponent },
    { path: 'group/:groupId', component: GroupClientsComponent }
];

export const appRoutingProviders: any[] = [

];

export const routing: ModuleWithProviders = RouterModule.forRoot(appRoutes);