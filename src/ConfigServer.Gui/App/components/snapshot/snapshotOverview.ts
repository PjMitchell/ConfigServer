import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { ConfigurationClientGroupDataService } from '../../dataservices/clientgroup-data.service';
import { SnapshotDataService } from '../../dataservices/snapshot-data.service';
import { IConfigurationClientGroup } from "../../interfaces/configurationClientGroup";
import { ISnapshotInfo } from "../../interfaces/snapshotInfo";
@Component({
    template: `
        <h3>Snapshots</h3>
        <group-header [csGroup]="group"></group-header>
        <hr />
        <div class="row">
            <div *ngFor="let snapshot of snapshots" class="col-sm-6 col-md-4" >
                <p>{{snapshot.name}}</p>
                <p>Created:{{snapshot.timeStamp | date:"MM/dd/yy" }}</p>
                <button type="button" class="btn btn-primary" (click)="deleteSnapshot(snapshot.id)"><span class="glyphicon-btn glyphicon glyphicon-trash"></span></button>
            </div>
        </div>
        <button type="button" class="btn btn-primary" (click)="back()">Back</button>
`,
})
export class SnapshotOverviewComponent implements OnInit {

    public snapshots: ISnapshotInfo[];
    public group: IConfigurationClientGroup;
    private groupId: string;
    constructor(private dataService: SnapshotDataService, private groupDataService: ConfigurationClientGroupDataService, private route: ActivatedRoute, private router: Router) {
        this.snapshots = new Array<ISnapshotInfo>();
        this.group = { groupId: '', name: '', imagePath: '' };
    }

    public ngOnInit() {
        this.route.params.forEach((value) => {
            this.groupId = value['groupId'];
            this.refreshSnapshotList();
            this.getGroupInfo();
        });
    }

    public back() {
        if (this.groupId) {
            this.router.navigate(['/group', this.groupId]);
        } else {
            this.router.navigate(['/group']);
        }
    }
    private async refreshSnapshotList() {
        this.snapshots = await this.dataService.getSnapshots(this.groupId);
    }

    private async deleteSnapshot(snapshotId: string) {
        await this.dataService.deleteSnapShot(snapshotId);
        await this.refreshSnapshotList();
    }

    private async getGroupInfo() {
        this.group = await this.groupDataService.getClientGroup(this.groupId);
    }
}
