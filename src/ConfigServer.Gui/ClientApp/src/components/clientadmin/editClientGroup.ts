import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { ConfigurationClientGroupDataService } from '../../dataservices/clientgroup-data.service';
import { IConfigurationClientGroup } from '../../interfaces/configurationClientGroup';

@Component({
    template: `
        <h2>Edit client group</h2>
        <div *ngIf="group">
            <edit-clientgroup-input [(csClientGroup)]="group" ></edit-clientgroup-input>
            <hr />
            <div>
               <button type="button" mat-raised-button color="primary" (click)="back()">Back</button>
               <button id="save-btn" type="button" mat-raised-button color="accent" [disabled]="isDisabled" (click)="save()">Save</button>
            </div>
        </div>
`,
})
export class EditClientGroupComponent implements OnInit {
    public group: IConfigurationClientGroup;
    public groupId: string;
    public isDisabled: boolean;
    constructor(private clientGroupDataService: ConfigurationClientGroupDataService, private route: ActivatedRoute, private router: Router) {
    }

    public ngOnInit() {
        this.route.params.forEach((value) => {
            this.groupId = value['groupId'];
            this.clientGroupDataService.getClientGroup(this.groupId)
                .then((returnedClientGroup) => this.group = returnedClientGroup);
        });
    }

    public save(): void {
        this.isDisabled = true;
        this.clientGroupDataService.postClientGroup(this.group)
            .then(() => this.back());
    }

    public back() {
        this.router.navigate(['/']);
    }
}
