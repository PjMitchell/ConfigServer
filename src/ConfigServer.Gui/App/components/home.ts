import { Component, OnInit } from '@angular/core';
import { ConfigurationClientDataService } from '../dataservices/client-data.service';
import { ConfigurationClient } from '../interfaces/client';
import { Group } from '../interfaces/configurationSetDefintion';
import { Router } from '@angular/router';

@Component({
    template: `
        <h2>Clients</h2>
        <button type="button" class="btn btn-success" (click)="createNew()">Create</button>
        <div class="break"></div>
        <div class="group" *ngFor="let group of clients">
            <h3>{{group.key}}</h3>
            <div class="item" *ngFor="let client of group.items">
                <h3>{{client.name}}</h3>
                <p>Id: {{client.clientId}}</p>
                <p>{{client.enviroment}}</p>
                <p>{{client.description}}</p>
                <button type="button" class="btn btn-primary" (click)="goToClient(client.clientId)">Manage configurations</button>
                <button type="button" class="btn btn-primary" (click)="editClient(client.clientId)">Edit client</button>
            </div>
        </div>
`
})
export class HomeComponent implements OnInit {
    clients: Group<string, ConfigurationClient>[];

    constructor(private clientDataService: ConfigurationClientDataService, private router: Router) {

    }

    ngOnInit(): void {
        this.clientDataService.getClients()
            .then(returnedClients => this.mapClients(returnedClients));
    }

    goToClient(clientId: string) {
        this.router.navigate(['/client', clientId]);
    }

    createNew() {
        this.router.navigate(['/createClient']);
    }

    mapClients(value: ConfigurationClient[]): void {
        var grouping = new Array<ConfigurationClient[]>();
        value.forEach((item) => {
            var group: ConfigurationClient[] = grouping[item.group];
            if (!group)
                group = new Array<ConfigurationClient>();

            group.push(item);
            grouping[item.group] = group
        });
        var keys = Object.keys(grouping);
        var items = new Array<Group<string, ConfigurationClient>>()
        for (var i = 0; i < keys.length; i++) {
            var key = keys[i];
            var val: ConfigurationClient[] = grouping[key];
            if (key == 'null')
                key = '';
            items.push({ 'key': key, 'items': val });
        }
        this.clients = items
    }

    editClient(clientId: string) {
        this.router.navigate(['/editClient', clientId]);
    }
}