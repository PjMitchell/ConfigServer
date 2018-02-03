import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { ConfigurationClientGroupDataService } from '../dataservices/clientgroup-data.service';
import { UserPermissionService } from '../dataservices/userpermission-data.service';
import { IConfigurationClient } from '../interfaces/configurationClient';
import { IConfigurationClientGroup } from '../interfaces/configurationClientGroup';

@Component({
    template: `
            <group-header [csGroup]="group"></group-header>
            <hr />
                <button type="button" mat-raised-button color="primary" (click)="goToSnapshots()">Snapshots</button>
            <hr />
            <div class="row">
                <div id="client-panel-{{client.clientId}}" class="col-sm-6 col-md-4 client-panel"  *ngFor="let client of clients">
                    <div  class="thumbnail">
                        <h3 class="client-name">{{client.name}}</h3>
                        <p class="client-id">Id: {{client.clientId}}</p>
                        <p class="client-enviroment">{{client.enviroment}}</p>
                        <p class="client-description">{{client.description}}</p>
                        <button id="manage-client-btn-{{client.clientId}}" type="button" mat-raised-button color="primary" (click)="goToClient(client.clientId)">Manage configurations</button>
                        <button id="edit-client-btn-{{client.clientId}}" *ngIf="canEditClients" type="button" mat-raised-button color="primary" (click)="editClient(client.clientId)">Edit client</button>
                    </div>
                </div>
            </div>
            <button type="button" mat-raised-button color="primary" (click)="back()">Back</button>
`,
})
export class GroupClientsComponent  implements OnInit {
    public groupId: string;
    public isSpecifiedGroup: boolean = false;
    public clients: IConfigurationClient[];
    public group: IConfigurationClientGroup;
    public canEditClients = false;
    constructor(private clientDataService: ConfigurationClientGroupDataService, private permissionService: UserPermissionService, private router: Router, private route: ActivatedRoute ) {
        this.clients = new Array<IConfigurationClient>();
        this.group = {
            groupId : '',
            name: 'loading...',
            imagePath: '',
        };
    }

    public ngOnInit(): void {
        this.route.params.forEach((value) => {
            this.groupId = value['groupId'];
        });
        if (!this.groupId) {
            this.clientDataService.getClientForGroup().then((result) => { this.clients = result; });
            this.isSpecifiedGroup = false;
            this.group = {
                        groupId : '',
                        name: 'No group',
                        imagePath: '',
            };
        } else {
            this.isSpecifiedGroup = true;
            this.clientDataService.getClientForGroup(this.groupId).then((result) => { this.clients = result; });
            this.clientDataService.getClientGroup(this.groupId).then((result) => { this.group = result; });
        }
        this.permissionService.getPermission()
            .then((permissions) => {
                this.canEditClients = permissions.canEditClients;
        });
    }

    public goToClient(clientId: string) {
        this.router.navigate(['/client', clientId]);
    }

    public editClient(clientId: string) {
        this.router.navigate(['/editClient', clientId]);
    }

    public goToSnapshots() {
        this.router.navigate(['/snapshots', this.groupId]);
    }

    public back() {
        this.router.navigate(['/']);
    }
}
