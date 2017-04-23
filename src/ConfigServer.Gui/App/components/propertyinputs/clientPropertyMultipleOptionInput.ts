import { Component, EventEmitter, Input, Output } from '@angular/core';
import { IConfigurationPropertyPayload } from "../../interfaces/configurationPropertyPayload";

@Component({
    selector: 'multiple-option-input',
    template: `
    <select class="form-control" multiple [(ngModel)]="csConfig[csDefinition.propertyName]">
        <option *ngFor="let p of csDefinition.options | toKeyValuePairs" [value]="p.key">{{p.value}}</option>
    </select>
`,
})
export class ConfigurationPropertyMultipleOptionInputComponent {
    @Input()
    public csDefinition: IConfigurationPropertyPayload;
    @Input()
    public csConfig: any;
    @Output()
    public csConfigChange: EventEmitter<any> = new EventEmitter<any>();
}
