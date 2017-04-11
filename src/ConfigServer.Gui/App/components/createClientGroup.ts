import { Component } from '@angular/core';
import { ConfigurationClientGroupDataService } from '../dataservices/clientgroup-data.service';

import { Router } from '@angular/router';
import { ConfigurationClient } from '../interfaces/configurationClient';
import { ConfigurationClientGroup } from '../interfaces/configurationClientGroup';



@Component({
    template: `
        <h2>Create client group</h2>
        <div>
            <edit-clientgroup-input [(csClientGroup)]="group"></edit-clientgroup-input>
            <hr />
            <div>
               <button type="button"  class="btn btn-primary"(click)="back()">Back</button>
               <button [disabled]="isDisabled" type="button" class="btn btn-success" (click)="create()">Create</button>
            </div>
        </div>
`
})
export class CreateClientGroupComponent {
    group: ConfigurationClientGroup;
    isDisabled: boolean;
    constructor(private clientGroupDataService: ConfigurationClientGroupDataService, private router: Router) {
        this.group = {
            groupId: '',
            name: '',
            imagePath: ''
        };
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