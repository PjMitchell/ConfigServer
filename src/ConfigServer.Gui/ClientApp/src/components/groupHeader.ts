﻿import { Component, Input, Output } from '@angular/core';
import { IConfigurationClientGroup } from "../interfaces/configurationClientGroup";

@Component({
    selector: 'group-header',
    template: `
        <div class="row">
            <div class="col-sm-6 col-md-4">
                <div *ngIf="group.imagePath"><img id="group-image" class="img-responsive" src="Resource/ClientGroupImages/{{group.imagePath}}" /></div>
            </div>
            <div class="col-sm-6 col-md-8">
                <h3 id="group-title">Group: {{group.name}}</h3>
                <h4 id="group-id">Id: {{group.groupId}}</h4>
            </div>
        </div>
`,
})
export class GroupHeaderComponent {
    @Input('csGroup')
    public group: IConfigurationClientGroup;
}
