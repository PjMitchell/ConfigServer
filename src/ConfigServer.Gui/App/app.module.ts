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

import { ConfigurationPropertyComponent } from './components/configProperty';
import { ConfigurationPropertyInputComponent } from './components/clientPropertyInput';
import { ConfigurationPropertyIntergerInputComponent } from './components/clientPropertyIntergerInput';
import { ConfigurationPropertyFloatInputComponent } from './components/clientPropertyFloatInput';
import { ConfigurationPropertyBoolInputComponent } from './components/clientPropertyBoolInput';
import { ConfigurationPropertyDateInputComponent } from './components/clientPropertyDateInput';
import { ConfigurationPropertyStringInputComponent } from './components/clientPropertyStringInput';
import { ConfigurationPropertyEnumInputComponent } from './components/clientPropertyEnumInput';
import { ConfigurationPropertyOptionInputComponent } from './components/clientPropertyOptionInput';
import { ConfigurationPropertyMultipleOptionInputComponent } from './components/clientPropertyMultipleOptionInput';
import { ConfigurationPropertyCollectionInputComponent } from './components/clientPropertyCollectionInput';
import { JsonFileUploaderComponent } from './components/jsonFileUploader';
import { CreateClientComponent } from './components/createClient';
import { EditClientInputComponent } from './components/editClientInput';
import { EditClientComponent } from './components/editClient';
import { ConfigurationOverviewComponent } from './components/configurationOverview';
import { ConfigurationSetComponent } from './components/configurationSetOverview';
import { ResourceOverviewComponent } from './components/resourceOverview';



import { ObjectToIteratorPipe, ObjectToKeyValuePairsPipe } from './pipes/objectToIterable';

import { ConfigurationClientDataService } from './dataservices/client-data.service';
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
        CreateClientComponent,
        EditClientInputComponent,
        EditClientComponent,
        JsonFileUploaderComponent,
        ResourceOverviewComponent,
        ObjectToIteratorPipe,
        ObjectToKeyValuePairsPipe
    ],
    bootstrap: [AppShell],
    providers: [appRoutingProviders, ConfigurationClientDataService, ConfigurationSetDataService, ResourceDataService, ConfigurationDataService, UploadDataService]
})

export class AppModule { }