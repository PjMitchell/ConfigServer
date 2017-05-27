
import { ModuleWithProviders } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { ClientConfigShellComponent } from './components/clientConfigShell';
import { ClientOverviewComponent } from './components/clientOverview';
import { ConfigArchiveComponent } from './components/configArchive';
import { CreateClientComponent } from './components/createClient';
import { CreateClientGroupComponent } from './components/createClientGroup';
import { EditClientComponent } from './components/editClient';
import { EditClientGroupComponent } from './components/editClientGroup';
import { GroupClientsComponent } from './components/groupClients';
import { HomeComponent } from './components/home';
import { ResourceArchiveComponent } from './components/resourceArchive';
const appRoutes: Routes = [
    { path: '', component: HomeComponent },
    { path: 'createClient', component: CreateClientComponent },
    { path: 'editClient/:clientId', component: EditClientComponent },
    { path: 'createClientGroup', component: CreateClientGroupComponent },
    { path: 'editClientGroup/:groupId', component: EditClientGroupComponent },
    { path: 'client/:clientId', component: ClientOverviewComponent },
    { path: 'client/:clientId/:configurationSetId/:configurationId', component: ClientConfigShellComponent },
    { path: 'group', component: GroupClientsComponent },
    { path: 'group/:groupId', component: GroupClientsComponent },
    { path: 'resourceArchive/:clientId', component: ResourceArchiveComponent },
    { path: 'configArchive/:clientId', component: ConfigArchiveComponent },
];

export const appRoutingProviders: any[] = [

];

export const routing: ModuleWithProviders = RouterModule.forRoot(appRoutes, { useHash: true });
