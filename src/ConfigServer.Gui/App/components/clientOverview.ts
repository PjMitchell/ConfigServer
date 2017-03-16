import { Component, OnInit } from '@angular/core';
import { ConfigurationClientDataService } from '../dataservices/client-data.service';
import { ConfigurationSetDataService } from '../dataservices/configset-data.service';
import { ResourceDataService } from '../dataservices/resource-data.service';

import { ActivatedRoute, Router } from '@angular/router';
import { ConfigurationClient } from '../interfaces/client';
import { ResourceInfo } from '../interfaces/resourceInfo';

import { ConfigurationSetSummary } from '../interfaces/configurationSetSummary';


@Component({
    template: `
        <div *ngIf="client">
            <h2>{{client.name}}</h2>
                <p>Id: {{client.clientId}}</p>
                <p>{{client.enviroment}}</p>
                <p>{{client.description}}</p>
        </div>

        <resource-overview  [csClientId]="clientId" [csResources]="resources" (onResourcesChanged)="onResourcesChanged($event)"></resource-overview>
 
        <h3>ConfigurationSets</h3>
        <div class="break">
        </div>
        <configSet-overview class="group" *ngFor="let configurationSet of configurationSets" [csClientId]="client.clientId" [csConfigurationSet]="configurationSet" >
        </configSet-overview>
        <button type="button" (click)="back()">Back</button>
`
})
export class ClientOverviewComponent implements OnInit {
    client: ConfigurationClient;
    clientId: string;
    configurationSets: ConfigurationSetSummary[];
    resources: ResourceInfo[];
    constructor(private clientDataService: ConfigurationClientDataService, private configSetDataService: ConfigurationSetDataService, private resourceDataService: ResourceDataService, private route: ActivatedRoute, private router: Router) {

    }

    ngOnInit(): void {
        this.route.params.forEach((value) => {
            this.clientId = value['clientId'];
            this.clientDataService.getClient(this.clientId)
                .then(returnedClient => this.client = returnedClient);
            this.configSetDataService.getConfigurationSets()
                .then(returnedConfigSet => this.configurationSets = returnedConfigSet);
            this.resourceDataService.getClientResourceInfo(this.clientId)
                .then(returnedResources => this.resources = returnedResources);
        });
    }
    onResourcesChanged() {
        this.resourceDataService.getClientResourceInfo(this.clientId)
            .then(returnedResources => this.resources = returnedResources);
    }
    back() {
        this.router.navigate(['/']);
    }
}