import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { ArchiveConfigService } from '../dataservices/archiveconfig-data.service';
import { UserPermissionService } from '../dataservices/userpermission-data.service';
import { IArchivedConfigInfo } from '../interfaces/archivedConfigInfo';
import { IGroup } from "../interfaces/group";

@Component({
    templateUrl: './config-archive.component.html',
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
