import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { ConfigurationClientGroupDataService } from '../../../dataservices/clientgroup-data.service';
import { GuidGenerator } from '../../../dataservices/guid-generator';
import { IConfigurationClientGroup } from '../../../interfaces/configurationClientGroup';

@Component({
    templateUrl: './create-client-group.component.html',
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
