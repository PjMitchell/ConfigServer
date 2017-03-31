import { Component, Input, Output, EventEmitter } from '@angular/core';
import { ConfigurationPropertyPayload } from '../../interfaces/configurationSetDefintion';


@Component({
    selector: 'float-input',
    template: `
    <input   [(ngModel)]="csConfig[csDefinition.propertyName]" step="0.00001" type="number" min="{{csDefinition.validationDefinition.min}}" max="{{csDefinition.validationDefinition.max}}" required="{{csDefinition.validationDefinition.isRequired}}">
`
})
export class ConfigurationPropertyFloatInputComponent {
    @Input()
    csDefinition: ConfigurationPropertyPayload;
    @Input()
    csConfig: any;
    @Output()
    csConfigChange: EventEmitter<any> = new EventEmitter<any>();
}