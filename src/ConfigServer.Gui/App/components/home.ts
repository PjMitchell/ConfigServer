import { Component, OnInit } from '@angular/core';
import { ConfigurationClientDataService } from '../dataservices/client-data.service';
import { IConfigurationClient } from '../interfaces/client';

@Component({
    template: `
        <H2>Clients</H2>
        <ul>
            <li *ngFor="let client of clients">{{client.name}} - {{client.description}}</li>
        </ul>`
})
export class HomeComponent implements OnInit {
    clients: IConfigurationClient[];

    constructor(private clientDataService: ConfigurationClientDataService)  {

    }

    ngOnInit(): void {
        this.clientDataService.getClients()
            .then(returnedClients => this.clients = returnedClients);
    }
}