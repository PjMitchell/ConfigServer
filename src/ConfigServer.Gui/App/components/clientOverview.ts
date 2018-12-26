﻿import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { ConfigurationClientDataService } from '../dataservices/client-data.service';
import { ConfigurationSetDataService } from '../dataservices/configset-data.service';
import { ResourceDataService } from '../dataservices/resource-data.service';
import { UserPermissionService } from '../dataservices/userpermission-data.service';
import { IConfigurationClient } from '../interfaces/configurationClient';
import { IConfigurationClientSetting } from '../interfaces/configurationClientSetting';
import { IConfigurationSetSummary } from '../interfaces/configurationSetSummary';
import { IResourceInfo } from '../interfaces/resourceInfo';
import { ITag } from '../interfaces/tag';

@Component({
    template: `
        <client-header [csClient]="client"></client-header>
        <resource-overview  [csClientId]="clientId" [csResources]="resources" (onResourcesChanged)="onResourcesChanged($event)" [csIsConfigurator]="isConfigurator"></resource-overview>
        <h3>ConfigurationSets</h3>
        <button type="button" mat-raised-button color="primary" (click)="goToArchive()">Archive</button>
        <snapshot-input [csClientId]="clientId"></snapshot-input>
        <button type="button" mat-raised-button color="primary" (click)="goToLoadSnapshot()">Load Snapshot</button>
        <div class="break"></div>
        <configSet-overview class="group" *ngFor="let configurationSet of configurationSets" [csClientId]="client.clientId" [csConfigurationSet]="configurationSet" >
        </configSet-overview>
        <button type="button" mat-raised-button color="primary" (click)="back()">Back</button>
`,
})
export class ClientOverviewComponent implements OnInit {
    public client: IConfigurationClient;
    public clientId: string;
    public isConfigurator = false;
    public configurationSets: IConfigurationSetSummary[];
    public resources: IResourceInfo[];
    constructor(private clientDataService: ConfigurationClientDataService, private configSetDataService: ConfigurationSetDataService, private resourceDataService: ResourceDataService, private permissionService: UserPermissionService, private route: ActivatedRoute, private router: Router) {
        this.clientId = '';
        this.client = {
            clientId: '',
            name: 'Loading...',
            enviroment: '',
            description: '',
            group: '',
            readClaim: '',
            configuratorClaim: '',
            settings: new Array<IConfigurationClientSetting>(),
            tags: new Array<ITag>(),
        };
        this.configurationSets = new Array<IConfigurationSetSummary>();
        this.resources = new Array<IResourceInfo>();
    }

    public ngOnInit(): void {
        this.route.params.forEach((value) => {
            this.clientId = value['clientId'];
            this.loadInitialData(this.clientId);
        });
    }
    public onResourcesChanged() {
        this.resourceDataService.getClientResourceInfo(this.clientId)
            .then((returnedResources) => this.resources = returnedResources);
    }

    public goToArchive() {
        this.router.navigate(['/configArchive', this.client.clientId]);
    }

    public goToLoadSnapshot() {
        this.router.navigate(['/pushSnapshots', this.client.clientId]);
    }

    public back() {
        if (this.client.group) {
            this.router.navigate(['/group', this.client.group]);
        } else {
            this.router.navigate(['/group']);
        }
    }

    private async loadInitialData(clientId: string) {
        this.client = await this.clientDataService.getClient(clientId);
        const permission = await this.permissionService.getPermission();
        if (!this.client.configuratorClaim) {
            this.isConfigurator = true;
        } else if (permission.clientConfiguratorClaims.length !== 0 &&  permission.clientConfiguratorClaims.some((value) => value === this.client.configuratorClaim)) {
            this.isConfigurator = true;
        } else {
            this.isConfigurator = false;
        }
        if (this.isConfigurator) {
            const unfilteredConfigurationSets = await this.configSetDataService.getConfigurationSets();
            this.configurationSets = unfilteredConfigurationSets.filter((value) => this.isAvailableConfigurationSetForClient(value, this.client));
            this.resources = await this.resourceDataService.getClientResourceInfo(clientId);
        }
    }
    private isAvailableConfigurationSetForClient(value: IConfigurationSetSummary, client: IConfigurationClient): any {
        if (value.requiredClientTag) {
            return client.tags.some((item) => item.value === value.requiredClientTag);
        }
        return true;
    }
}
