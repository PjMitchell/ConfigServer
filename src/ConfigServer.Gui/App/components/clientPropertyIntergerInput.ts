import { Component, Input, Output, EventEmitter } from '@angular/core';
import { ConfigurationPropertyPayload } from '../interfaces/configurationSetDefintion';


@Component({
    selector: 'interger-input',
    template: `<input [(ngModel)]="csConfig[csDefinition.propertyName]" type="number" min="{{csDefinition.validationDefinition.min}}" max="{{csDefinition.validationDefinition.max}}">`
})
export class ConfigurationPropertyIntergerInputComponent {
    @Input()
    csDefinition: ConfigurationPropertyPayload;
    @Input()
    csConfig: any;
    @Output()
    csConfigChange: EventEmitter<any> = new EventEmitter<any>();
}