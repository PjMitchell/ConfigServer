import { Component, Input, Output, EventEmitter } from '@angular/core';
import { ConfigurationClient } from '../interfaces/client';


@Component({
    selector: 'edit-client-input',
    template: `
    <h4>Name:</h4>
    <input [(ngModel)]="csClient.name" type="text">
    <h4>Description:</h4>
    <input [(ngModel)]="csClient.description" type="text">
`

})
export class EditClientInputComponent {
    @Input()
    csClient: ConfigurationClient;
    @Output()
    csClientChange: EventEmitter<ConfigurationClient> = new EventEmitter<ConfigurationClient>();
}