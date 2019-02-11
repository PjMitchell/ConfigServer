
import { ModuleWithProviders } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { CreateClientComponent } from './components/clientadmin/createClient';
import { CreateClientGroupComponent } from './components/clientadmin/createClientGroup';
import { EditClientComponent } from './components/clientadmin/editClient';
import { EditClientGroupComponent } from './components/clientadmin/editClientGroup';
import { ClientConfigShellComponent } from './components/clientConfigShell';
import { ClientOverviewComponent } from './components/clientOverview';
import { ConfigArchiveComponent } from './components/configArchive';
import { CopyResourceComponent } from "./components/copyResource";
import { GroupClientsComponent } from './components/groupClients';
import { HomeComponent } from './components/home';
import { ResourceArchiveComponent } from './components/resourceArchive';
import { PushSnapshotComponent } from "./components/snapshot/pushSnapshot";
import { SnapshotOverviewComponent } from './components/snapshot/snapshotOverview';

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
    { path: 'copyResource/:clientId', component: CopyResourceComponent },
    { path: 'snapshots/:groupId', component: SnapshotOverviewComponent },
    { path: 'pushSnapshots/:clientId', component: PushSnapshotComponent },

];

export const appRoutingProviders: any[] = [

];

export const routing: ModuleWithProviders = RouterModule.forRoot(appRoutes, { useHash: true });
