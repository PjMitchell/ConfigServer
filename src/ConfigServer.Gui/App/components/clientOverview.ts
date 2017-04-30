import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { ConfigurationClientDataService } from '../dataservices/client-data.service';
import { ConfigurationSetDataService } from '../dataservices/configset-data.service';
import { ResourceDataService } from '../dataservices/resource-data.service';
import { IConfigurationClient } from '../interfaces/configurationClient';
import { IConfigurationClientSetting } from '../interfaces/configurationClientSetting';
import { IConfigurationSetSummary } from '../interfaces/configurationSetSummary';
import { IResourceInfo } from '../interfaces/resourceInfo';

@Component({
    template: `
        <div *ngIf="client">
            <h2 id="client-name">{{client.name}}</h2>
            <p id="client-id">Id: {{client.clientId}}</p>
            <p id="client-env">{{client.enviroment}}</p>
            <p id="client-desc">{{client.description}}</p>
        </div>

        <resource-overview  [csClientId]="clientId" [csResources]="resources" (onResourcesChanged)="onResourcesChanged($event)"></resource-overview>

        <h3>ConfigurationSets</h3>
        <div class="break">
        </div>
        <configSet-overview class="group" *ngFor="let configurationSet of configurationSets" [csClientId]="client.clientId" [csConfigurationSet]="configurationSet" >
        </configSet-overview>
        <button type="button" class="btn btn-primary" (click)="back()">Back</button>
`,
})
export class ClientOverviewComponent implements OnInit {
    public client: IConfigurationClient;
    public clientId: string;
    public configurationSets: IConfigurationSetSummary[];
    public resources: IResourceInfo[];
    constructor(private clientDataService: ConfigurationClientDataService, private configSetDataService: ConfigurationSetDataService, private resourceDataService: ResourceDataService, private route: ActivatedRoute, private router: Router) {
        this.clientId = '';
        this.client = {
            clientId: '',
            name: 'Loading...',
            enviroment: '',
            description: '',
            group: '',
            settings: new Array<IConfigurationClientSetting>(),
        };
        this.configurationSets = new Array<IConfigurationSetSummary>();
        this.resources = new Array<IResourceInfo>();
    }

    public ngOnInit(): void {
        this.route.params.forEach((value) => {
            this.clientId = value['clientId'];
            this.clientDataService.getClient(this.clientId)
                .then((returnedClient) => this.client = returnedClient);
            this.configSetDataService.getConfigurationSets()
                .then((returnedConfigSet) => this.configurationSets = returnedConfigSet);
            this.resourceDataService.getClientResourceInfo(this.clientId)
                .then((returnedResources) => this.resources = returnedResources);
        });
    }
    public onResourcesChanged() {
        this.resourceDataService.getClientResourceInfo(this.clientId)
            .then((returnedResources) => this.resources = returnedResources);
    }
    public back() {
        if (this.client.group) {
            this.router.navigate(['/group', this.client.group]);
        } else {
            this.router.navigate(['/group']);
        }
    }
}
