﻿import { Component, OnInit } from '@angular/core';
import { ConfigurationClientGroupDataService } from '../dataservices/clientGroup-data.service';
import { ConfigurationClientGroup } from '../interfaces/configurationClientGroup';
import { Group } from '../interfaces/configurationSetDefintion';
import { Router } from '@angular/router';

@Component({
    template: `
    <button type="button" class="btn btn-primary" (click)="createNewClient()"><span class="glyphicon glyphicon-plus"></span> Add Client</button>
    <button type="button" class="btn btn-primary" (click)="createNewGroup()"><span class="glyphicon glyphicon-plus"></span> Add Group</button>
    <hr />
    <div class="row">
        <div class="col-sm-6 col-md-4"  *ngFor="let group of groups">
            <div class="thumbnail">
                <div><img class="img-responsive" src="group.imagePath" /></div>
                <div class="category"></div>
                <div class="caption">
                    <h3>{{group.name}}</h3>
                    <p>{{group.groupId}}</p>
                    <hr />
                    <button type="button" class="btn btn-primary" (click)="editGroupClients(group)">Edit</button>
                </div>
            </div>
        </div>
    </div>
    <button type="button" class="btn btn-primary" (click)="editClientsWithNoGroup()">Edit clients with not in group</button>
`
})
export class HomeComponent implements OnInit {
    groups: ConfigurationClientGroup[];

    constructor(private clientDataService: ConfigurationClientGroupDataService, private router: Router) {
        this.groups = new Array<ConfigurationClientGroup>();
    }

    ngOnInit(): void {
        this.clientDataService.getClientGroups()
            .then(returnedClientGroups => { this.groups = returnedClientGroups});
    }

    createNewClient() {
        this.router.navigate(['/createClient']);
    }
    editClientsWithNoGroup() {
        this.router.navigate(['/group']);
    }
    editGroupClients(selectedGroup: ConfigurationClientGroup) {
        this.router.navigate(['/group', selectedGroup.groupId]);
    }
}