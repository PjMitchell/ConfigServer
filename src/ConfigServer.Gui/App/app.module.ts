import { NgModule } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { HttpModule } from '@angular/http';
import { BrowserModule } from '@angular/platform-browser';
import { appRoutingProviders, routing } from './app.routing';
import { AppShell } from './app.shell';
import { ClientConfigShellComponent } from './components/clientConfigShell';
import { ConfigurationInputComponent } from './components/clientConfigurationInput';
import { OptionInputComponent } from './components/clientOptionInput';
import { ClientOverviewComponent } from './components/clientOverview';
import { ConfigArchiveComponent } from "./components/configArchive";
import { EditClientSettingInputComponent } from './components/configClientSettingInput';
import { ConfigurationOverviewComponent } from './components/configurationOverview';
import { ConfigurationSetComponent } from './components/configurationSetOverview';
import { CopyResourceComponent } from "./components/copyResource";
import { CreateClientComponent } from './components/createClient';
import { CreateClientGroupComponent } from './components/createClientGroup';
import { EditClientComponent } from './components/editClient';
import { EditClientGroupComponent } from './components/editClientGroup';
import { EditClientGroupInputComponent } from './components/editClientGroupInput';
import { EditClientInputComponent } from './components/editClientInput';
import { GroupClientsComponent } from './components/groupClients';
import { HomeComponent } from './components/home';
import { ConfigurationPropertyBoolInputComponent } from './components/propertyinputs/clientPropertyBoolInput';
import { ConfigurationPropertyCollectionInputComponent } from './components/propertyinputs/clientPropertyCollectionInput';
import { ConfigurationPropertyDateInputComponent } from './components/propertyinputs/clientPropertyDateInput';
import { ConfigurationPropertyEnumInputComponent } from './components/propertyinputs/clientPropertyEnumInput';
import { ConfigurationPropertyFloatInputComponent } from './components/propertyinputs/clientPropertyFloatInput';
import { ConfigurationPropertyInputComponent } from './components/propertyinputs/clientPropertyInput';
import { ConfigurationPropertyIntergerInputComponent } from './components/propertyinputs/clientPropertyIntergerInput';
import { ConfigurationPropertyMultipleOptionInputComponent } from './components/propertyinputs/clientPropertyMultipleOptionInput';
import { ConfigurationPropertyOptionInputComponent } from './components/propertyinputs/clientPropertyOptionInput';
import { ConfigurationPropertyStringInputComponent } from './components/propertyinputs/clientPropertyStringInput';
import { ConfigurationPropertyComponent } from './components/propertyinputs/configProperty';
import { ResourceArchiveComponent } from './components/resourceArchive';
import { ResourceOverviewComponent } from './components/resourceOverview';
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
import { UploadDataService } from './dataservices/upload-data.service';
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
        ObjectToIteratorPipe,
        ObjectToKeyValuePairsPipe,
    ],
    imports: [BrowserModule, routing, HttpModule, FormsModule],
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
        ],
})

export class AppModule { }
