import { Component, OnInit } from '@angular/core';
import { ConfigurationClientGroupDataService } from '../dataservices/clientgroup-data.service';
import { GuidGenerator } from '../dataservices/guid-generator';
import { Router } from '@angular/router';
import { ConfigurationClient } from '../interfaces/configurationClient';
import { ConfigurationClientGroup } from '../interfaces/configurationClientGroup';



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
`
})
export class CreateClientGroupComponent implements OnInit {
    group: ConfigurationClientGroup;
    isDisabled: boolean;
    constructor(private clientGroupDataService: ConfigurationClientGroupDataService, private guidGenerator: GuidGenerator, private router: Router) {
        this.group = {
            groupId: '',
            name: '',
            imagePath: ''
        };
    }

    ngOnInit() {
        this.isDisabled == true;
        this.guidGenerator.getGuid()
            .then(s => {
                this.group.groupId = s;
                this.isDisabled = false;
            })
    }

    create(): void {
        this.isDisabled;
        this.clientGroupDataService.postClientGroup(this.group)
            .then(() => this.back());
    }

    back() {
        this.router.navigate(['/']);
    }
}