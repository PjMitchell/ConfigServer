import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { ConfigurationClientGroupDataService } from '../dataservices/clientgroup-data.service';
import { UserPermissionService } from '../dataservices/userpermission-data.service';
import { IConfigurationClientGroup } from '../interfaces/configurationClientGroup';

@Component({
    templateUrl: './home.component.html',
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
