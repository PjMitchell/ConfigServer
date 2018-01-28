import { Component, EventEmitter, Input, Output } from '@angular/core';
import { IConfigurationPropertyPayload } from "../../interfaces/configurationPropertyPayload";

@Component({
    selector: 'enum-input',
    template: `
    <mat-form-field class="full-width">
        <mat-select placeholder="{{csDefinition.propertyDisplayName}}" [(value)]="csConfig[csDefinition.propertyName]">
            <mat-option *ngFor="let p of csDefinition.options | toKeyValuePairs" [value]="p.key">{{p.value}}</mat-option>
        </mat-select>
        <mat-hint>{{csDefinition.propertyDescription}}</mat-hint>
    </mat-form-field>
`,
})
export class ConfigurationPropertyEnumInputComponent {
    @Input()
    public csDefinition: IConfigurationPropertyPayload;
    @Input()
    public csConfig: any;
    @Output()
    public csConfigChange: EventEmitter<any> = new EventEmitter<any>();
}
