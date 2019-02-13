import { NgModule } from '@angular/core';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { HttpModule } from '@angular/http';
import { MatAutocompleteModule, MatButtonModule, MatDatepickerModule, MatFormFieldModule, MatInputModule, MatNativeDateModule, MatOptionModule, MatSelectModule } from '@angular/material';
import { BrowserModule } from '@angular/platform-browser';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { appRoutingProviders, routing } from './app.routing';
import { AppShell } from './app.shell';
import { DeleteBeforeComponent } from './components/buttons/delete-before-button.component';
import { IconButtonComponent } from './components/buttons/icon-button.component';
import { ClientConfigShellComponent } from './components/client-config-shell.component';
import { ConfigurationInputComponent } from './components/client-configuration-input.component';
import { ClientHeaderComponent } from "./components/client-header.component";
import { OptionInputComponent } from './components/client-option-input.component';
import { ClientOverviewComponent } from './components/client-overview.component';
import { CreateClientGroupComponent } from './components/clientadmin/client-group/create-client-group.component';
import { EditClientGroupInputComponent } from './components/clientadmin/client-group/edit-client-group-input.component';
import { EditClientGroupComponent } from './components/clientadmin/client-group/edit-client-group.component';
import { EditClientSettingInputComponent } from './components/clientadmin/client/client-setting-input.component';
import { CreateClientComponent } from './components/clientadmin/client/create-client.component';
import { EditClientInputComponent } from './components/clientadmin/client/edit-client-input.component';
import { EditClientComponent } from './components/clientadmin/client/edit-client.component';
import { ConfigArchiveComponent } from './components/config-archive.component';
import { ConfigurationOverviewComponent } from './components/configuration-overview.component';
import { ConfigurationSetComponent } from './components/configurationset-overview.component';
import { CopyResourceComponent } from './components/copy-resource.component';
import { GroupClientsComponent } from './components/group-clients.component';
import { GroupHeaderComponent } from "./components/group-header.component";
import { HomeComponent } from './components/home.component';
import { ConfigurationPropertyBoolInputComponent } from './components/property-inputs/bool-input.component';
import { ConfigurationPropertyClassInputComponent } from './components/property-inputs/class-input.component';
import { ConfigurationPropertyCollectionInputComponent } from './components/property-inputs/collection-input.component';
import { ConfigurationPropertyInputComponent } from './components/property-inputs/config-property-item.component';
import { ConfigurationPropertyComponent } from './components/property-inputs/config-property.component';
import { ConfigurationPropertyDateInputComponent } from './components/property-inputs/date-input.component';
import { ConfigurationPropertyEnumInputComponent } from './components/property-inputs/enum-input.component';
import { ConfigurationPropertyFloatInputComponent } from './components/property-inputs/float-input.component';
import { ConfigurationPropertyIntergerCollectionInputComponent } from './components/property-inputs/interger-collection-input.component';
import { ConfigurationPropertyIntergerInputComponent } from './components/property-inputs/interger-input.component';
import { ConfigurationPropertyMultipleOptionInputComponent} from './components/property-inputs/multiple-option-input.component';
import { ConfigurationPropertyOptionInputComponent } from './components/property-inputs/option-input.component';
import { ConfigurationPropertyStringCollectionInputComponent } from './components/property-inputs/string-collection-input.component';
import { ConfigurationPropertyStringInputComponent } from './components/property-inputs/string-input.component';
import { ResourceArchiveComponent } from './components/resource-archive.component';
import { ResourceOverviewComponent } from './components/resource-overview.component';
import { PushSnapshotComponent } from "./components/snapshot/push-snapshot.component";
import { SaveSnapshotInputComponent } from './components/snapshot/save-snapshot-input.component';
import { SnapshotOverviewComponent } from "./components/snapshot/snapshot-overview.component";
import { GroupImageFileUploaderComponent } from './components/uploaders/group-image-uploader.component';
import { JsonFileUploaderComponent } from './components/uploaders/json-file-uploader.component';
import { ResourceFileUploaderComponent } from './components/uploaders/resource-file-uploader.component';
import { ArchiveConfigService } from './dataservices/archiveconfig-data.service';
import { ConfigurationClientDataService } from './dataservices/client-data.service';
import { ConfigurationClientGroupDataService } from './dataservices/clientgroup-data.service';
import { ConfigurationDataService } from './dataservices/config-data.service';
import { ConfigurationSetDataService } from './dataservices/configset-data.service';
import { GuidGenerator } from './dataservices/guid-generator';
import { ResourceDataService } from './dataservices/resource-data.service';
import { SnapshotDataService } from "./dataservices/snapshot-data.service";
import { TagDataService } from './dataservices/tag-data.service';
import { UploadDataService } from './dataservices/upload-data.service';
import { UserPermissionService } from './dataservices/userpermission-data.service';
import { ObjectToIteratorPipe } from './pipes/objectToIterable';
import { ObjectToKeyValuePairsPipe } from './pipes/objectToKeyValuePairsPipe';

@NgModule({
    bootstrap: [AppShell],
    declarations: [
        AppShell,
        HomeComponent,
        ClientOverviewComponent,
        ConfigurationSetComponent,
        ConfigurationOverviewComponent,
        ClientConfigShellComponent,
        ConfigurationInputComponent,
        OptionInputComponent,
        EditClientSettingInputComponent,
        ConfigurationPropertyComponent,
        ConfigurationPropertyInputComponent,
        ConfigurationPropertyIntergerInputComponent,
        ConfigurationPropertyFloatInputComponent,
        ConfigurationPropertyBoolInputComponent,
        ConfigurationPropertyStringInputComponent,
        ConfigurationPropertyDateInputComponent,
        ConfigurationPropertyEnumInputComponent,
        ConfigurationPropertyOptionInputComponent,
        ConfigurationPropertyMultipleOptionInputComponent,
        ConfigurationPropertyCollectionInputComponent,
        ConfigurationPropertyClassInputComponent,
        ConfigurationPropertyIntergerCollectionInputComponent,
        ConfigurationPropertyStringCollectionInputComponent,
        GroupClientsComponent,
        ResourceFileUploaderComponent,
        GroupImageFileUploaderComponent,
        CreateClientComponent,
        EditClientInputComponent,
        EditClientComponent,
        CreateClientGroupComponent,
        EditClientGroupInputComponent,
        EditClientGroupComponent,
        JsonFileUploaderComponent,
        ResourceOverviewComponent,
        ResourceArchiveComponent,
        ConfigArchiveComponent,
        CopyResourceComponent,
        SaveSnapshotInputComponent,
        SnapshotOverviewComponent,
        PushSnapshotComponent,
        GroupHeaderComponent,
        ClientHeaderComponent,
        ObjectToIteratorPipe,
        ObjectToKeyValuePairsPipe,
        IconButtonComponent,
        DeleteBeforeComponent,
    ],
    imports: [
        BrowserModule,
        routing,
        HttpModule,
        FormsModule,
        MatFormFieldModule,
        MatInputModule,
        BrowserAnimationsModule,
        MatOptionModule,
        MatSelectModule,
        MatAutocompleteModule,
        MatDatepickerModule,
        MatNativeDateModule,
        MatButtonModule,
        ReactiveFormsModule],
    providers: [
        appRoutingProviders,
        ArchiveConfigService,
        ConfigurationClientDataService,
        ConfigurationClientGroupDataService,
        ConfigurationDataService,
        ConfigurationSetDataService,
        GuidGenerator,
        ResourceDataService,
        UploadDataService,
        UserPermissionService,
        SnapshotDataService,
        TagDataService,
        ],
})

export class AppModule { }
