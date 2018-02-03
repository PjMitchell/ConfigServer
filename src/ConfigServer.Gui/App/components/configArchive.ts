import { Component, EventEmitter, Input, OnInit, Output, ViewChild } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { ArchiveConfigService } from '../dataservices/archiveconfig-data.service';
import { UserPermissionService } from '../dataservices/userpermission-data.service';
import { IArchivedConfigInfo } from '../interfaces/archivedConfigInfo';
import { IGroup } from "../interfaces/group";
import { IChildElement } from '../interfaces/htmlInterfaces';

@Component({
    template: `
        <div class="row">
            <h3>Config Archive</h3>
        </div>
        <div class="row" *ngFor="let group of groupedConfigs">
            <div class="row">
                <h4>{{group.key}}</h4>
            </div>
            <div class="row" groupedConfigs>
                <div *ngFor="let config of group.items" class="col-sm-6 col-md-4" >
                    <h5>{{config.name}}</h5>
                    <p>
                        ServerVersion:{{config.serverVersion}} <br/>
                        Created:{{config.timeStamp | date:"MM/dd/yy" }} <br/>
                        Archived:{{config.archiveTimeStamp | date:"MM/dd/yy" }}
                    </p>
                    <app-icon-button color="primary" *ngIf="canDownloadArchive" (click)="downloadConfig(config.name)"><span class="glyphicon-btn glyphicon glyphicon-download-alt"></span></app-icon-button>
                    <app-icon-button color="primary" *ngIf="canDeleteArchives" (click)="delete(config.name)"><span class="glyphicon-btn glyphicon glyphicon-trash"></span></app-icon-button>
                </div>
            </div>
        </div>
        <hr />
        <div class="row" *ngIf="canDeleteArchives">
            <app-delete-before-button class="col-sm-4 col-md-3" (onDeleteBefore)="deleteBefore($event)"></app-delete-before-button>
        </div>
        <hr />
        <div class="row">
            <button type="button" mat-raised-button color="primary" (click)="back()">Back</button>
        </div>
`,
})
export class ConfigArchiveComponent implements OnInit {
    public groupedConfigs: Array<IGroup<string, IArchivedConfigInfo>>;
    public clientId: string;
    public canDeleteArchives = false;
    public canDownloadArchive = false;
    constructor(private dataService: ArchiveConfigService, private permissionService: UserPermissionService, private route: ActivatedRoute, private router: Router) {
    }

    public ngOnInit() {
        this.route.params.forEach((value) => {
            this.clientId = value['clientId'];
            this.updateArchiveList();
        });
        this.permissionService.getPermissionForClient(this.clientId)
            .then((permissions) => {
                this.canDeleteArchives = permissions.canDeleteArchives;
                this.canDownloadArchive = permissions.hasClientConfiguratorClaim;
            });
    }

    public downloadConfig(file: string) {
        window.open('Archive/' + this.clientId + '/' + file);
    }

    public async delete(file: string) {
        const result = confirm('Are you sure you want to delete ' + file + '?');
        if (!result) {
            return;
        }
        await this.dataService.deleteArchivedConfig(this.clientId, file);
        await this.updateArchiveList();
    }

    public async deleteBefore(date: Date) {
        await this.dataService.deleteArchivedConfigBefore(this.clientId, date);
        await this.updateArchiveList();
    }

    public back() {
        this.router.navigate(['/client/' + this.clientId]);
    }

    private async updateArchiveList() {
        const configs = await this.dataService.getArchivedConfig(this.clientId);
        this.groupedConfigs = this.groupArchivedConfigsByConfigName(configs);
    }

    private groupArchivedConfigsByConfigName(configs: IArchivedConfigInfo[]) {
        const group = {};
        configs.forEach((value) => {
            if (!group[value.configuration]) {
                group[value.configuration] = new Array<IArchivedConfigInfo>();
            }
            group[value.configuration].push(value);
        });
        const result = new Array<IGroup<string, IArchivedConfigInfo>>();
        for (const propertyName in group) {
            if (group.hasOwnProperty(propertyName)) {
                result.push({key: propertyName, items: group[propertyName]});
            }
        }
        return result;
    }
}
