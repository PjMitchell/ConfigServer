﻿import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { ConfigurationClientGroupDataService } from '../dataservices/clientgroup-data.service';
import { UserPermissionService } from '../dataservices/userpermission-data.service';
import { IConfigurationClientGroup } from '../interfaces/configurationClientGroup';

@Component({
    template: `
    <button id="createClientBtn" *ngIf="canEditClients" type="button" mat-raised-button color="primary" (click)="createNewClient()"><span class="glyphicon glyphicon-plus"></span> Add Client</button>
    <button id="createGroupBtn" *ngIf="canEditGroups" type="button" mat-raised-button color="primary" (click)="createNewGroup()"><span class="glyphicon glyphicon-plus"></span> Add Group</button>
    <hr />
    <div class="row">
        <div id="group-panel-{{group.groupId}}" class="col-sm-6 col-md-4 group-panel"  *ngFor="let group of groups">
            <div class="thumbnail">
                <div *ngIf="group.imagePath"><img class="img-responsive home-group-img" src="Resource/ClientGroupImages/{{group.imagePath}}" /></div>
                <div class="category"></div>
                <div class="caption">
                    <h3 class="home-group-name">{{group.name}}</h3>
                    <p class="home-group-id">{{group.groupId}}</p>
                    <hr />
                    <button id="manage-group-btn-{{group.groupId}}" type="button" mat-raised-button color="primary" (click)="manageGroupClients(group)">Manage group</button>
                    <button id="edit-group-btn-{{group.groupId}}" *ngIf="canEditGroups" type="button" mat-raised-button color="primary" (click)="editGroup(group)">Edit group</button>
                </div>
            </div>
        </div>
    </div>
    <button id="manage-group-btn" type="button" mat-raised-button color="primary" (click)="manageClientsWithNoGroup()">Manage clients not in group</button>
`,
})
export class HomeComponent implements OnInit {
    public groups: IConfigurationClientGroup[];
    public canEditGroups = false;
    public canEditClients = false;

    constructor(private clientDataService: ConfigurationClientGroupDataService, private permissionService: UserPermissionService, private router: Router) {
        this.groups = new Array<IConfigurationClientGroup>();
    }

    public ngOnInit(): void {
        this.clientDataService.getClientGroups()
            .then((returnedClientGroups) => { this.groups = returnedClientGroups; });
        this.permissionService.getPermission()
            .then((permissions) => {
                this.canEditClients = permissions.canEditClients;
                this.canEditGroups = permissions.canEditGroups;
            });
    }

    public createNewClient() {
        this.router.navigate(['/createClient']);
    }

    public createNewGroup() {
        this.router.navigate(['/createClientGroup']);
    }

    public editGroup(selectedGroup: IConfigurationClientGroup) {
        this.router.navigate(['/editClientGroup', selectedGroup.groupId]);
    }

    public manageClientsWithNoGroup() {
        this.router.navigate(['/group']);
    }
    public manageGroupClients(selectedGroup: IConfigurationClientGroup) {
        this.router.navigate(['/group', selectedGroup.groupId]);
    }
}
