import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { ResourceDataService } from '../dataservices/resource-data.service';
import { UserPermissionService } from '../dataservices/userpermission-data.service';
import { IResourceInfo } from '../interfaces/resourceInfo';

@Component({
    templateUrl: './resource-archive.component.html',
})
export class ResourceArchiveComponent implements OnInit {
    public resources: IResourceInfo[];
    public clientId: string;
    public canDeleteArchives = false;
    public canDownloadArchive = false;
    constructor(private dataService: ResourceDataService, private permissionService: UserPermissionService, private route: ActivatedRoute, private router: Router) {
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

    public downloadResource(file: string) {
        window.open('ResourceArchive/' + this.clientId + '/' + file);
    }

    public delete(file: string) {
        const result = confirm('Are you sure you want to delete ' + file + '?');
        if (!result) {
            return;
        }
        this.dataService.deleteArchivedResource(this.clientId, file).then(() => this.updateArchiveList());
    }

    public deleteBefore(date: Date) {
        this.dataService.deleteArchivedResourceBefore(this.clientId, date).then(() => this.updateArchiveList());
    }

    public back() {
        this.router.navigate(['/client/' + this.clientId]);
    }

    private updateArchiveList() {
        this.dataService.getClientArchiveResourceInfo(this.clientId)
            .then((returnedResources) => this.resources = returnedResources);
    }
}
