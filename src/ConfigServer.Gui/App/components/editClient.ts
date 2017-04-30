import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { ConfigurationClientDataService } from '../dataservices/client-data.service';
import { ConfigurationClientGroupDataService } from '../dataservices/clientgroup-data.service';
import { IConfigurationClient } from '../interfaces/configurationClient';
import { IConfigurationClientGroup } from '../interfaces/configurationClientGroup';

@Component({
    template: `
        <h2>Edit client</h2>
        <h4 id="client-id" *ngIf="client">{{client.clientId}}</h4>
        <div *ngIf="client && groups">
            <edit-client-input [csAllClient]="clients" [(csClient)]="client" [csExistingGroups]="groups" [(csIsValid)]="isValid"></edit-client-input>
            <hr />
            <div>
               <button type="button" class="btn btn-primary" (click)="back()">Back</button>
               <button id="save-btn" type="button" class="btn btn-success" [disabled]="isDisabled || !isValid" (click)="save()">Save</button>
            </div>
        </div>
`,
})
export class EditClientComponent implements OnInit {
    public client: IConfigurationClient;
    public clients: IConfigurationClient[];
    public groups: IConfigurationClientGroup[];
    public clientId: string;
    public isDisabled: boolean = false;
    public isValid: boolean = true;
    constructor(private clientDataService: ConfigurationClientDataService, private clientGroupDataService: ConfigurationClientGroupDataService, private route: ActivatedRoute, private router: Router) {
    }

    public ngOnInit() {
        this.route.params.forEach((value) => {
            this.clientId = value['clientId'];
            this.clientDataService.getClients()
                .then((returnedClient) => this.onAllClientsReturned(returnedClient));
        });
        this.clientGroupDataService.getClientGroups().then((grp) => {this.groups = grp; });
    }

    public onAllClientsReturned(returnedClients: IConfigurationClient[]) {
        this.clients = returnedClients;
        returnedClients.filter((value) => value.clientId === this.clientId).forEach((value) => {
            this.client = value;
        });
    }

    public save(): void {
        this.isDisabled = true;
        this.clientDataService.postClient(this.client)
            .then(() => this.back());
    }

    public back() {
        this.router.navigate(['/']);
    }
}
