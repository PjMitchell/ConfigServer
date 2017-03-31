import { Component, Input, Output, EventEmitter } from '@angular/core';
import { ConfigurationPropertyPayload } from '../../interfaces/configurationSetDefintion';


@Component({
    selector: 'multiple-option-input',
    template: `
    <select multiple [(ngModel)]="csConfig[csDefinition.propertyName]">
        <option *ngFor="let p of csDefinition.options | toKeyValuePairs" [value]="p.key">{{p.value}}</option>
    </select>
`
})
export class ConfigurationPropertyMultipleOptionInputComponent {
    @Input()
    csDefinition: ConfigurationPropertyPayload;
    @Input()
    csConfig: any;
    @Output()
    csConfigChange: EventEmitter<any> = new EventEmitter<any>();
}