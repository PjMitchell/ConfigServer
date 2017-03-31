import { Component } from '@angular/core';
import { ConfigurationClientDataService } from '../dataservices/client-data.service';
import { ConfigurationClientGroupDataService } from '../dataservices/clientGroup-data.service';

import { Router } from '@angular/router';
import { ConfigurationClient } from '../interfaces/configurationClient';
import { ConfigurationClientGroup } from '../interfaces/configurationClientGroup';



@Component({
    template: `
        <h2>Create client</h2>
        <div>
            <edit-client-input [csAllClient]="clients" [(csClient)]="client" [csExistingGroups]="groups"></edit-client-input>
            <hr />
            <div>
               <button type="button"  class="btn btn-primary"(click)="back()">Back</button>
               <button [disabled]="isDisabled" type="button" class="btn btn-success" (click)="create()">Create</button>
            </div>
        </div>
`
})
export class CreateClientComponent {
    client: ConfigurationClient;
    clients: ConfigurationClient[];
    groups: ConfigurationClientGroup[];
    isDisabled: boolean;
    constructor(private clientDataService: ConfigurationClientDataService,private clientGroupDataService: ConfigurationClientGroupDataService, private router: Router) {
        this.client = {
            clientId: '',
            name: '',
            group: '',
            enviroment: '',
            description: ''
        };
    }

    ngOnInit() {
       this.clientDataService.getClients()
            .then(returnedClient => this.onAllClientsReturned(returnedClient));
       this.clientGroupDataService.getClientGroups().then(grp => {this.groups = grp;})
    }

    onAllClientsReturned(returnedClients: ConfigurationClient[]) {
        this.clients = returnedClients;
    }

    create(): void {
        this.isDisabled;
        this.clientDataService.postClient(this.client)
            .then(() => this.back());
    }

    back() {
        this.router.navigate(['/']);
    }
}