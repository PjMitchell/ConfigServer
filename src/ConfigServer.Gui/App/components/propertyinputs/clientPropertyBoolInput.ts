import { Component, EventEmitter, Input, Output } from '@angular/core';
import { IConfigurationPropertyPayload } from "../../interfaces/configurationPropertyPayload";

@Component({
    selector: 'bool-input',
    template: `
    <mat-form-field class="full-width">
        <mat-select placeholder="{{csDefinition.propertyDisplayName}}" [(value)]="csConfig[csDefinition.propertyName]">
            <mat-option [value]="true">True</mat-option>
            <mat-option [value]="false">False</mat-option>
        </mat-select>
        <mat-hint>{{csDefinition.propertyDescription}}</mat-hint>
    </mat-form-field>
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
