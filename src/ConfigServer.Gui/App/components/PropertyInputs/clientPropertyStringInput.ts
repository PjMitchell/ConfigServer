import { Component, Input, Output, EventEmitter } from '@angular/core';
import { ConfigurationPropertyPayload } from '../../interfaces/configurationSetDefintion';


@Component({
    selector: 'string-input',
    template: `<input class="form-control" [(ngModel)]="csConfig[csDefinition.propertyName]" type="text" pattern="{{csDefinition.validationDefinition.pattern}}" maxlength="{{csDefinition.validationDefinition.maxLength}}">`
})
export class ConfigurationPropertyStringInputComponent {
    @Input()
    csDefinition: ConfigurationPropertyPayload;
    @Input()
    csConfig: any;
    @Output()
    csConfigChange: EventEmitter<string> = new EventEmitter<any>();
}