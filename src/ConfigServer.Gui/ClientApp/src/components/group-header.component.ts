import { Component, Input, Output } from '@angular/core';
import { IConfigurationClientGroup } from "../interfaces/configurationClientGroup";

@Component({
    selector: 'group-header',
    templateUrl: './group-header.component.html',
})
export class GroupHeaderComponent {
    @Input('csGroup')
    public group: IConfigurationClientGroup;
}
