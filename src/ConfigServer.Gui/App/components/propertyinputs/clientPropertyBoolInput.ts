import { Component, EventEmitter, Input, Output } from '@angular/core';
import { IConfigurationPropertyPayload } from "../../interfaces/configurationPropertyPayload";

@Component({
    selector: 'bool-input',
    template: `
    <select class="form-control" [(ngModel)]="csConfig[csDefinition.propertyName]">
        <option [value]="true">True</option>
        <option [value]="false">False</option>
    </select>
`,
})
export class ConfigurationPropertyBoolInputComponent {
    @Input()
    public csDefinition: IConfigurationPropertyPayload;
    @Input()
    public csConfig: any;
    @Output()
    public csConfigChange: EventEmitter<any> = new EventEmitter<any>();
}
