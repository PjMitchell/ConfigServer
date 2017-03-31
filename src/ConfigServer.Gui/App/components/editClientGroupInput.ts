import { Component, Input, Output, EventEmitter } from '@angular/core';
import { ConfigurationClientGroup } from '../interfaces/configurationClientGroup';



@Component({
    selector: 'edit-clientgroup-input',
    template: `
    <h4>Name:</h4>
    <input [(ngModel)]="csClientGroup.name" type="text">    
`

})
export class EditClientGroupInputComponent {
    @Input()
    csClientGroup: ConfigurationClientGroup;
    @Output()
    csClientGroupChange: EventEmitter<ConfigurationClientGroup> = new EventEmitter<ConfigurationClientGroup>();
}