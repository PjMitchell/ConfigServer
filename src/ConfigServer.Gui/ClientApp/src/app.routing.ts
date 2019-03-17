
import { ModuleWithProviders } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { ClientConfigShellComponent } from './components/client-config-shell.component';
import { ClientOverviewComponent } from './components/client-overview.component';
import { CreateClientGroupComponent } from './components/clientadmin/client-group/create-client-group.component';
import { EditClientGroupComponent } from './components/clientadmin/client-group/edit-client-group.component';
import { CreateClientComponent } from './components/clientadmin/client/create-client.component';
import { EditClientComponent } from './components/clientadmin/client/edit-client.component';
import { ConfigArchiveComponent } from './components/config-archive.component';
import { CopyResourceComponent } from "./components/copy-resource.component";
import { GroupClientsComponent } from './components/group-clients.component';
import { HomeComponent } from './components/home.component';
import { ResourceArchiveComponent } from './components/resource-archive.component';
import { PushSnapshotComponent } from "./components/snapshot/push-snapshot.component";
import { SnapshotOverviewComponent } from './components/snapshot/snapshot-overview.component';

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
