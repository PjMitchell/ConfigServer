import { NgModule } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { HttpModule } from '@angular/http';
import { MatAutocompleteModule, MatButtonModule, MatDatepickerModule, MatFormFieldModule, MatInputModule, MatNativeDateModule, MatOptionModule, MatSelectModule } from '@angular/material';
import { BrowserModule } from '@angular/platform-browser';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { appRoutingProviders, routing } from './app.routing';
import { AppShell } from './app.shell';
import { DeleteBeforeComponent } from './components/buttons/deleteBeforeButton.component';
import { IconButtonComponent } from './components/buttons/iconButton.component';
import { EditClientSettingInputComponent } from './components/clientadmin/configClientSettingInput';
import { CreateClientComponent } from './components/clientadmin/createClient';
import { CreateClientGroupComponent } from './components/clientadmin/createClientGroup';
import { EditClientComponent } from './components/clientadmin/editClient';
import { EditClientGroupComponent } from './components/clientadmin/editClientGroup';
import { EditClientGroupInputComponent } from './components/clientadmin/editClientGroupInput';
import { EditClientInputComponent } from './components/clientadmin/editClientInput';
import { ClientConfigShellComponent } from './components/clientConfigShell';
import { ConfigurationInputComponent } from './components/clientConfigurationInput';
import { ClientHeaderComponent } from "./components/clientHeader";
import { OptionInputComponent } from './components/clientOptionInput';
import { ClientOverviewComponent } from './components/clientOverview';
import { ConfigArchiveComponent } from './components/configArchive';
import { ConfigurationOverviewComponent } from './components/configurationOverview';
import { ConfigurationSetComponent } from './components/configurationSetOverview';
import { CopyResourceComponent } from './components/copyResource';
import { GroupClientsComponent } from './components/groupClients';
import { GroupHeaderComponent } from "./components/groupHeader";
import { HomeComponent } from './components/home';
import { ConfigurationPropertyBoolInputComponent } from './components/propertyinputs/clientPropertyBoolInput';
import { ConfigurationPropertyCollectionInputComponent } from './components/propertyinputs/clientPropertyCollectionInput';
import { ConfigurationPropertyDateInputComponent } from './components/propertyinputs/clientPropertyDateInput';
import { ConfigurationPropertyEnumInputComponent } from './components/propertyinputs/clientPropertyEnumInput';
import { ConfigurationPropertyFloatInputComponent } from './components/propertyinputs/clientPropertyFloatInput';
import { ConfigurationPropertyInputComponent } from './components/propertyinputs/clientPropertyInput';
import { ConfigurationPropertyIntergerInputComponent } from './components/propertyinputs/clientPropertyIntergerInput';
import { ConfigurationPropertyMultipleOptionInputComponent} from './components/propertyinputs/clientPropertyMultipleOptionInput';
import { ConfigurationPropertyOptionInputComponent } from './components/propertyinputs/clientPropertyOptionInput';
import { ConfigurationPropertyStringInputComponent } from './components/propertyinputs/clientPropertyStringInput';
import { ConfigurationPropertyComponent } from './components/propertyinputs/configProperty';
import { ResourceArchiveComponent } from './components/resourceArchive';
import { ResourceOverviewComponent } from './components/resourceOverview';
import { PushSnapshotComponent } from "./components/snapshot/pushSnapshot";
import { SaveSnapshotInputComponent } from './components/snapshot/saveSnapshotInput';
import { SnapshotOverviewComponent } from "./components/snapshot/snapshotOverview";
import { GroupImageFileUploaderComponent } from './components/uploaders/groupImageUploader';
import { JsonFileUploaderComponent } from './components/uploaders/jsonFileUploader';
import { ResourceFileUploaderComponent } from './components/uploaders/resourceFileUploader';
import { ArchiveConfigService } from './dataservices/archiveconfig-data.service';
import { ConfigurationClientDataService } from './dataservices/client-data.service';
import { ConfigurationClientGroupDataService } from './dataservices/clientgroup-data.service';
import { ConfigurationDataService } from './dataservices/config-data.service';
import { ConfigurationSetDataService } from './dataservices/configset-data.service';
import { GuidGenerator } from './dataservices/guid-generator';
import { ResourceDataService } from './dataservices/resource-data.service';
import { SnapshotDataService } from "./dataservices/snapshot-data.service";
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
        MatButtonModule],
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
        ],
})

export class AppModule { }
