import { Component, OnInit } from '@angular/core';
import { ConfigurationClientDataService } from '../dataservices/client-data.service';
import { IConfigurationClient } from '../interfaces/client';
import { Router } from '@angular/router';
@Component({
    template: `
        <h2>Clients</h2>
        <div *ngFor="let client of clients">
            <h3>{{client.name}}</h3>
            <p>{{client.description}}</p>
            <button type="button" (click)="goToClient(client.clientId)">Manage</button>
        </div>
`
})
export class HomeComponent implements OnInit {
    clients: IConfigurationClient[];

    constructor(private clientDataService: ConfigurationClientDataService, private router: Router) {

    }

    ngOnInit(): void {
        this.clientDataService.getClients()
            .then(returnedClients => this.clients = returnedClients);
    }

    goToClient(clientId: string) {
        this.router.navigate(['/client', clientId])
    }

}