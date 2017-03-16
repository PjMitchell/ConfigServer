import { Component, OnInit } from '@angular/core';
import { ConfigurationClientDataService } from '../dataservices/client-data.service';
import { ConfigurationClient } from '../interfaces/client';
import { Group } from '../interfaces/configurationSetDefintion';
import { Router } from '@angular/router';
import { GroupTransitService } from '../dataservices/group-transit.service';

@Component({
    template: `
        <div class="container">
            <h3>Group: {{group.key}}</h3>
            <hr />
            <div class="row">
                <div class="col-sm-6 col-md-4"  *ngFor="let client of group.items">
                    <div  class="thumbnail">
                        <h3>{{client.name}}</h3>
                        <p>Id: {{client.clientId}}</p>
                        <p>{{client.enviroment}}</p>
                        <p>{{client.description}}</p>
                        <button type="button" class="btn btn-primary" (click)="goToClient(client.clientId)">Manage configurations</button>
                        <button type="button" class="btn btn-primary" (click)="editClient(client.clientId)">Edit client</button>
                    </div>
                </div>
            </div>
        </div>
`
})
export class EditGroupClientsComponent {
    group: Group<string, ConfigurationClient>;

    constructor(private clientDataService: ConfigurationClientDataService, private router: Router, private groupTransitService: GroupTransitService) {
        this.group = groupTransitService.selectedGroup;
    }

    goToClient(clientId: string) {
        this.router.navigate(['/client', clientId]);
    }

    editClient(clientId: string) {
        this.router.navigate(['/editClient', clientId]);
    }
}