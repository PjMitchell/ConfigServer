import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { ConfigurationClientGroupDataService } from '../dataservices/clientgroup-data.service';
import { UserPermissionService } from '../dataservices/userpermission-data.service';
import { IConfigurationClient } from '../interfaces/configurationClient';
import { IConfigurationClientGroup } from '../interfaces/configurationClientGroup';

@Component({
    templateUrl: './group-clients.component.html',
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
