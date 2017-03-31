import { Component, Input, Output, EventEmitter } from '@angular/core';
import { ConfigurationPropertyPayload } from '../../interfaces/configurationSetDefintion';


@Component({
    selector: 'bool-input',
    template: `
    <select class="form-control" [(ngModel)]="csConfig[csDefinition.propertyName]">
        <option [value]="true">True</option>
        <option [value]="false">False</option>
    </select>
`
})
export class ConfigurationPropertyBoolInputComponent {
    @Input()
    csDefinition: ConfigurationPropertyPayload;
    @Input()
    csConfig: any;
    @Output()
    csConfigChange: EventEmitter<any> = new EventEmitter<any>();
}