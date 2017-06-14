import { Component, EventEmitter, Input, OnInit, Output, ViewChild } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { timestamp } from 'rxjs/operator/timestamp';
import { ArchiveConfigService } from '../dataservices/archiveconfig-data.service';
import { ConfigurationClientDataService } from "../dataservices/client-data.service";
import { ResourceDataService } from '../dataservices/resource-data.service';
import { UserPermissionService } from "../dataservices/userpermission-data.service";
import { IArchivedConfigInfo } from '../interfaces/archivedConfigInfo';
import { IConfigurationClient } from "../interfaces/configurationClient";
import { IGroup } from "../interfaces/group";
import { IChildElement } from '../interfaces/htmlInterfaces';
import { ISelectableResourceInfo } from '../interfaces/selectableResourceInfo';

@Component({
    template: `
        <div class="row">
            <h3>Copy Resources</h3>
        </div>
        <div class="row" *ngIf="isClientInfoReady">
            <div *ngIf="sourceClient" class="col-sm-4 col-md-4">
                <h4>Source</h4>
                <h5 id="client-name">{{sourceClient.name}}</h5>
                <p id="client-id">Id: {{sourceClient.clientId}}</p>
                <p id="client-env">{{sourceClient.enviroment}}</p>
                <p id="client-desc">{{sourceClient.description}}</p>
            </div>
            <div  *ngIf="clients.length > 0"   class="col-sm-4 col-md-4">
                <select [(ngModel)]="targetClient"  class="form-control">
                    <option *ngFor="let client of clients" [ngValue]="client">{{client.name}}</option>
                </select>
            </div>
            <div  *ngIf="clients.length === 0"   class="col-sm-4 col-md-4">
                <p>No targets found</p>
            </div>
            <div *ngIf="targetClient" class="col-sm-4 col-md-4">
                <h4>Target</h4>
                <h5 id="client-name">{{targetClient.name}}</h5>
                <p id="client-id">Id: {{targetClient.clientId}}</p>
                <p id="client-env">{{targetClient.enviroment}}</p>
                <p id="client-desc">{{targetClient.description}}</p>
            </div>
        </div>
        <hr />
        <div class="row">
            <div *ngFor="let resource of resources" class="col-sm-6 col-md-4 selectable-panel" [ngClass]="{'selectable-panel-selected': resource.isSelected}"
 (click)="select(resource)">
                <p>{{resource.name}}</p>
                <p>Created:{{resource.timeStamp | date:"MM/dd/yy" }}</p>
            </div>
        </div>
        <hr />
        <div class="row">
            <button type="button" class="btn btn-primary" (click)="back()">Back</button> <button type="button" class="btn btn-primary" [disabled]="isDisabled" (click)="copy()">Copy to target</button>
        </div>
`,
})
export class CopyResourceComponent implements OnInit {
    public clients: IConfigurationClient[];
    public resources: ISelectableResourceInfo[];
    public clientId: string;
    public sourceClient: IConfigurationClient;
    public targetClient: IConfigurationClient;
    public isClientInfoReady = false;
    public isDisabled = true;

    constructor(private clientDataService: ConfigurationClientDataService, private permissionService: UserPermissionService, private resourceService: ResourceDataService, private route: ActivatedRoute, private router: Router) {
        this.clients = new Array<IConfigurationClient>();
    }

    public ngOnInit() {
        this.route.params.forEach((value) => {
            this.clientId = value['clientId'];
            this.isClientInfoReady = false;
            this.buildClientList();
            this.buildResourceList();
        });
    }

    public select(resouce: ISelectableResourceInfo) {
        resouce.isSelected = !resouce.isSelected;
    }

    public async copy() {
        this.isDisabled = true;
        const resourcesToCopy = this.resources.filter((value) => value.isSelected).map((value) => value.name);
        if (this.targetClient && resourcesToCopy.length > 0) {
            await this.resourceService.copyResources(this.clientId, this.targetClient.clientId, resourcesToCopy);
            this.router.navigate(['/client/' + this.targetClient.clientId]);
        }
    }

    public back() {
        this.router.navigate(['/client/' + this.clientId]);
    }

    private async buildClientList() {
        const permission = await this.permissionService.getPermission();
        const allClients = await this.clientDataService.getClients();
        this.sourceClient = allClients.find((value) => value.clientId === this.clientId);

        if (this.sourceClient) {
            this.clients = allClients.filter((value) => value.group === this.sourceClient.group && value.clientId !== this.sourceClient.clientId && this.canConfigureClient(value.configuratorClaim, permission.clientConfiguratorClaims));
        }
        if (this.clients.length > 0) {
            this.targetClient = this.clients[0];
            this.isDisabled = false;
        }
        this.isClientInfoReady = true;
    }

    private async buildResourceList() {
        const allResources = await this.resourceService.getClientResourceInfo(this.clientId);
        this.resources = allResources.map((value) => {
            return {
            isSelected: false,
            name: value.name,
            timeStamp: value.timeStamp,
            };
        });
    }

    private canConfigureClient(clientClaim: string, permissions: string[]) {
        if (!clientClaim) {
            return true;
        }
        return permissions.some((claim) => claim === clientClaim);
    }
}
