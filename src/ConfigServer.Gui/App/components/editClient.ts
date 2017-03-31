import { Component, OnInit } from '@angular/core';
import { ConfigurationClientDataService } from '../dataservices/client-data.service';
import { ConfigurationClientGroupDataService } from '../dataservices/clientGroup-data.service';

import { Router, ActivatedRoute,  } from '@angular/router';
import { ConfigurationClient } from '../interfaces/configurationClient';
import { ConfigurationClientGroup } from '../interfaces/configurationClientGroup';



@Component({
    template: `
        <h2>Edit client</h2>
        <div *ngIf="client && groups">
            <edit-client-input [csAllClient]="clients" [(csClient)]="client" [csExistingGroups]="groups"></edit-client-input>
            <hr />
            <div>
               <button type="button" class="btn btn-primary" (click)="back()">Back</button>
               <button type="button" class="btn btn-success" [disabled]="isDisabled" (click)="save()">Save</button>
            </div>
        </div>
`
})
export class EditClientComponent implements OnInit {
    client: ConfigurationClient;
    clients: ConfigurationClient[];
    groups: ConfigurationClientGroup[];
    clientId: string;
    isDisabled: boolean;
    constructor(private clientDataService: ConfigurationClientDataService,private clientGroupDataService: ConfigurationClientGroupDataService, private route: ActivatedRoute, private router: Router) {
    }

    ngOnInit() {
        this.route.params.forEach((value) => {
            this.clientId = value['clientId'];
            this.clientDataService.getClients()
                .then(returnedClient => this.onAllClientsReturned(returnedClient));
        });
        this.clientGroupDataService.getClientGroups().then(grp => {this.groups = grp;})
    }

    onAllClientsReturned(returnedClients: ConfigurationClient[]) {
        this.clients = returnedClients
        returnedClients.filter((value) => value.clientId === this.clientId).forEach((value) => {
            this.client = value;
        });
    }

    save(): void {
        this.isDisabled = true;
        this.clientDataService.postClient(this.client)
            .then(() => this.back());
    }



    back() {
        this.router.navigate(['/']);
    }
}