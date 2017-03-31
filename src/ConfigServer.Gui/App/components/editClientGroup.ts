import { Component, OnInit } from '@angular/core';
import { ConfigurationClientGroupDataService } from '../dataservices/clientGroup-data.service';
import { Router, ActivatedRoute,  } from '@angular/router';
import { ConfigurationClientGroup } from '../interfaces/configurationClientGroup';

@Component({
    template: `
        <h2>Edit client group</h2>
        <div *ngIf="group">
            <edit-clientgroup-input [(csClientGroup)]="group" ></edit-clientgroup-input>
            <hr />
            <div>
               <button type="button" class="btn btn-primary" (click)="back()">Back</button>
               <button type="button" class="btn btn-success" [disabled]="isDisabled" (click)="save()">Save</button>
            </div>
        </div>
`
})
export class EditClientGroupComponent implements OnInit {
    group: ConfigurationClientGroup;
    groupId: string;
    isDisabled: boolean;
    constructor(private clientGroupDataService: ConfigurationClientGroupDataService, private route: ActivatedRoute, private router: Router) {
    }

    ngOnInit() {
        this.route.params.forEach((value) => {
            this.groupId = value['groupId'];
            this.clientGroupDataService.getClientGroup(this.groupId)
                .then(returnedClientGroup => this.group = returnedClientGroup);
        });
    }
    
    save(): void {
        this.isDisabled = true;
        this.clientGroupDataService.postClientGroup(this.group)
            .then(() => this.back());
    }
    
    back() {
        this.router.navigate(['/']);
    }
}