import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { ConfigurationClientGroupDataService } from '../dataservices/clientgroup-data.service';
import { GuidGenerator } from '../dataservices/guid-generator';
import { IConfigurationClient } from '../interfaces/configurationClient';
import { IConfigurationClientGroup } from '../interfaces/configurationClientGroup';

@Component({
    template: `
        <h2>Create client group</h2>
        <h4 id="groupid">{{group.groupId}}</h4>
        <div>
            <edit-clientgroup-input [(csClientGroup)]="group"></edit-clientgroup-input>
            <hr />
            <div>
               <button type="button"  class="btn btn-primary"(click)="back()">Back</button>
               <button id="save-btn" [disabled]="isDisabled" type="button" class="btn btn-success" (click)="create()">Create</button>
            </div>
        </div>
`,
})
export class CreateClientGroupComponent implements OnInit {
    public group: IConfigurationClientGroup;
    public isDisabled: boolean;
    constructor(private clientGroupDataService: ConfigurationClientGroupDataService, private guidGenerator: GuidGenerator, private router: Router) {
        this.group = {
            groupId: '',
            name: '',
            imagePath: '',
        };
    }

    public ngOnInit() {
        this.isDisabled = true;
        this.guidGenerator.getGuid()
            .then((s) => {
                this.group.groupId = s;
                this.isDisabled = false;
            });
    }

    public create(): void {
        this.isDisabled = true;
        this.clientGroupDataService.postClientGroup(this.group)
            .then(() => this.back());
    }

    public back() {
        this.router.navigate(['/']);
    }
}
