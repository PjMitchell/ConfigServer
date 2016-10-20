import { NgModule } from '@angular/core';
import { HttpModule } from '@angular/http';
import { FormsModule } from '@angular/forms';
import { BrowserModule } from '@angular/platform-browser';

import { routing, appRoutingProviders } from './app.routing'
import { AppShell } from './app.shell';
import { HomeComponent } from './components/home';
import { ClientComponent } from './components/client';
import { ClientConfigComponent } from './components/clientConfig';
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

import { ObjectToIteratorPipe, ObjectToKeyValuePairsPipe } from './pipes/objectToIterable';

import { ConfigurationClientDataService } from './dataservices/client-data.service';
import { ConfigurationSetDataService } from './dataservices/configset-data.service';
import { ConfigurationDataService } from './dataservices/config-data.service';

@NgModule({
    imports: [BrowserModule, routing, HttpModule, FormsModule],
    declarations: [
        AppShell,
        HomeComponent,
        ClientComponent,
        ClientConfigComponent,
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
        ObjectToIteratorPipe,
        ObjectToKeyValuePairsPipe
    ],
    bootstrap: [AppShell],
    providers: [appRoutingProviders, ConfigurationClientDataService, ConfigurationSetDataService, ConfigurationDataService]
})

export class AppModule { }