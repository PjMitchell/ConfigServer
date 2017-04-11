import { NgModule } from '@angular/core';
import { HttpModule } from '@angular/http';
import { FormsModule } from '@angular/forms';
import { BrowserModule } from '@angular/platform-browser';

import { routing, appRoutingProviders } from './app.routing';
import { AppShell } from './app.shell';
import { HomeComponent } from './components/home';
import { ClientOverviewComponent } from './components/clientOverview';
import { ClientConfigShellComponent } from './components/clientConfigShell';
import { ConfigurationInputComponent } from './components/clientConfigurationInput';
import { OptionInputComponent } from './components/clientOptionInput';
import { ConfigurationPropertyComponent } from './components/propertyinputs/configProperty';
import { ConfigurationPropertyInputComponent } from './components/propertyinputs/clientPropertyInput';
import { ConfigurationPropertyIntergerInputComponent } from './components/propertyinputs/clientPropertyIntergerInput';
import { ConfigurationPropertyFloatInputComponent } from './components/propertyinputs/clientPropertyFloatInput';
import { ConfigurationPropertyBoolInputComponent } from './components/propertyinputs/clientPropertyBoolInput';
import { ConfigurationPropertyDateInputComponent } from './components/propertyinputs/clientPropertyDateInput';
import { ConfigurationPropertyStringInputComponent } from './components/propertyinputs/clientPropertyStringInput';
import { ConfigurationPropertyEnumInputComponent } from './components/propertyinputs/clientPropertyEnumInput';
import { ConfigurationPropertyOptionInputComponent } from './components/propertyinputs/clientPropertyOptionInput';
import { ConfigurationPropertyMultipleOptionInputComponent } from './components/propertyinputs/clientPropertyMultipleOptionInput';
import { ConfigurationPropertyCollectionInputComponent } from './components/propertyinputs/clientPropertyCollectionInput';

import { CreateClientComponent } from './components/createClient';
import { EditClientInputComponent } from './components/editClientInput';
import { EditClientSettingInputComponent } from './components/configClientSettingInput';

import { EditClientComponent } from './components/editClient';
import { CreateClientGroupComponent } from './components/createClientGroup';
import { EditClientGroupInputComponent } from './components/editClientGroupInput';
import { EditClientGroupComponent } from './components/editClientGroup';
import { ConfigurationOverviewComponent } from './components/configurationOverview';
import { ConfigurationSetComponent } from './components/configurationSetOverview';
import { ResourceOverviewComponent } from './components/resourceOverview';
import { ResourceFileUploaderComponent } from './components/uploaders/resourceFileUploader';
import { GroupImageFileUploaderComponent } from './components/uploaders/groupImageUploader';
import { JsonFileUploaderComponent } from './components/uploaders/jsonFileUploader';
import { GroupClientsComponent } from './components/groupClients';



import { ObjectToIteratorPipe, ObjectToKeyValuePairsPipe } from './pipes/objectToIterable';

import { ConfigurationClientDataService } from './dataservices/client-data.service';
import { ConfigurationClientGroupDataService } from './dataservices/clientgroup-data.service';

import { ConfigurationSetDataService } from './dataservices/configset-data.service';
import { ConfigurationDataService } from './dataservices/config-data.service';
import { UploadDataService } from './dataservices/upload-data.service';
import { ResourceDataService } from './dataservices/resource-data.service';



@NgModule({
    imports: [BrowserModule, routing, HttpModule, FormsModule],
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
        ObjectToIteratorPipe,
        ObjectToKeyValuePairsPipe
    ],
    bootstrap: [AppShell],
    providers: [appRoutingProviders, ConfigurationClientDataService, ConfigurationSetDataService, ResourceDataService, ConfigurationDataService, UploadDataService, ConfigurationClientGroupDataService]
})

export class AppModule { }