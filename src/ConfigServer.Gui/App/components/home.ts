import { Component, OnInit } from '@angular/core';
import { ConfigurationClientDataService } from '../dataservices/client-data.service';
import { ConfigurationClient } from '../interfaces/client';

import { Router } from '@angular/router';
@Component({
    template: `
        <h2>Clients</h2>
        <button type="button" (click)="createNew()">Create</button>
        <div *ngFor="let client of clients">
            <h3>{{client.name}}</h3>
            <p>{{client.description}}</p>
            <button type="button" (click)="goToClient(client.clientId)">Manage configurations</button>
            <button type="button" (click)="editClient(client.clientId)">Edit client</button>
        </div>
`
})
export class HomeComponent implements OnInit {
    clients: ConfigurationClient[];

    constructor(private clientDataService: ConfigurationClientDataService, private router: Router) {

    }

    ngOnInit(): void {
        this.clientDataService.getClients()
            .then(returnedClients => this.clients = returnedClients);
    }

    goToClient(clientId: string) {
        this.router.navigate(['/client', clientId])
    }

    createNew() {
        this.router.navigate(['/createClient'])
    }

    editClient(clientId: string) {
        this.router.navigate(['/editClient', clientId])
    }
}