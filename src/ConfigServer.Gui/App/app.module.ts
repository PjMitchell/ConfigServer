import { NgModule } from '@angular/core';
import { HttpModule } from '@angular/http';

import { BrowserModule } from '@angular/platform-browser';
import { routing, appRoutingProviders } from './app.routing'
import { AppShell } from './app.shell';
import { HomeComponent } from './components/home';
import { ClientComponent } from './components/client';
import { ConfigurationClientDataService } from './dataservices/client-data.service';

@NgModule({
    imports: [BrowserModule, routing, HttpModule],
    declarations: [AppShell, HomeComponent, ClientComponent],
    bootstrap: [AppShell],
    providers: [appRoutingProviders, ConfigurationClientDataService]
})

export class AppModule { }