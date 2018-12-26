import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { ConfigurationClientDataService } from "../../dataservices/client-data.service";
import { ConfigurationClientGroupDataService } from '../../dataservices/clientgroup-data.service';
import { ConfigurationSetDataService } from "../../dataservices/configset-data.service";
import { SnapshotDataService } from '../../dataservices/snapshot-data.service';
import { IConfigurationClient } from "../../interfaces/configurationClient";
import { IConfigurationClientGroup } from "../../interfaces/configurationClientGroup";
import { IConfigurationClientSetting } from "../../interfaces/configurationClientSetting";
import { IConfigurationSetSummary } from "../../interfaces/configurationSetSummary";
import { ISelectableConfigurationModelSummary } from "../../interfaces/selectableConfigurationModelSummary";
import { ISnapshotInfo } from "../../interfaces/snapshotInfo";
import { Tag } from '../../interfaces/tag';

@Component({
    template: `
        <div  class="row">
            <client-header [csClient]="client"></client-header>
            <h3>Load snapshot</h3>
            <mat-form-field id="snapshot-input" class="full-width">
                <mat-select [(value)]="selectedSnapshot">
                    <mat-option *ngFor="let snapshot of snapshots" [value]="snapshot">{{snapshot.name}}</mat-option>
                </mat-select>
            </mat-form-field>
            <div *ngIf="selectedSnapshot" >
                <p>Created:{{selectedSnapshot.timeStamp | date:"MM/dd/yy" }}</p>
            </div>
            <h3>Configurations to be updated</h3>
        </div>
        <div  *ngFor="let configurationSet of configurationSets">
            <div  class="row">
                <h3>{{configurationSet.name}} <button type="button" mat-raised-button color="primary" (click)="toggleConfigSet(configurationSet)">All</button></h3>
            </div>
            <div class="row">
                <div *ngFor="let config of configurationSet.configs" class="col-sm-6 col-md-4 selectable-panel" [ngClass]="{'selectable-panel-selected': config.isSelected}"
    (click)="select(config)">
                    <p>{{config.displayName}}</p>
                    <p>{{config.description}}</p>
                </div>
            </div>
        </div>
        <hr />
        <div  class="row">
            <button type="button" mat-raised-button color="primary" (click)="pushSnapshot()"  [disabled]="!canPushSnapShot || !selectedSnapshot">Push snaphot to client</button>
            <button type="button" mat-raised-button color="primary" (click)="back()">Back</button>
        </div>
`,
})
export class PushSnapshotComponent implements OnInit {

    public snapshots: ISnapshotInfo[];
    public selectedSnapshot: ISnapshotInfo;
    public configurationSets: IConfigurationSetSummary[];
    public client: IConfigurationClient;
    private clientId: string;
    private canPushSnapShot: boolean;
    constructor(private dataService: SnapshotDataService, private clientDataService: ConfigurationClientDataService, private configSetDataService: ConfigurationSetDataService, private route: ActivatedRoute, private router: Router) {
        this.snapshots = new Array<ISnapshotInfo>();
        this.configurationSets = new Array<IConfigurationSetSummary>();
        this.client = { clientId: '', group: '', name: '', description: '', enviroment: '', readClaim: '', configuratorClaim: '', settings: new Array<IConfigurationClientSetting>(), tags: new Array<Tag>() };
        this.canPushSnapShot = false;
    }

    public ngOnInit() {
        this.route.params.forEach((value) => {
            this.clientId = value['clientId'];
            this.canPushSnapShot = false;
            this.loadInitialData();
        });
    }

    public async pushSnapshot() {
        this.canPushSnapShot = false;
        const selectedConfigs = this.flaternConfigs(this.configurationSets).filter((value) => value.isSelected).map((value) => value.id);
        await this.dataService.pushSnapshotToClient(this.selectedSnapshot.id, this.clientId, selectedConfigs);
        this.back();
    }

    public back() {
        this.router.navigate(['/client', this.clientId]);
    }

    public toggleConfigSet(configSet: IConfigurationSetSummary) {
        const result = configSet.configs.some((config) => !(config as ISelectableConfigurationModelSummary).isSelected);
        configSet.configs.forEach((config) => {
            (config as ISelectableConfigurationModelSummary).isSelected = result;
        });
    }

    private async loadInitialData() {
        this.client = await this.clientDataService.getClient(this.clientId);
        this.snapshots = await this.dataService.getSnapshots(this.client.group);
        const configurationSetModels = await this.configSetDataService.getConfigurationSets();
        configurationSetModels.forEach((configSet) => configSet.configs.forEach((config) => (config as ISelectableConfigurationModelSummary).isSelected = true));
        this.configurationSets = configurationSetModels;
        this.canPushSnapShot = true;
    }

    private flaternConfigs(values: IConfigurationSetSummary[]) {
        const result = new Array<ISelectableConfigurationModelSummary>();
        values.forEach((value) => value.configs.forEach((config) => result.push(config as ISelectableConfigurationModelSummary)));
        return result;
    }

    private select(summary: ISelectableConfigurationModelSummary) {
        summary.isSelected = !summary.isSelected;
    }
}
