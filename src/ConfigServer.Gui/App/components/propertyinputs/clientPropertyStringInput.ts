import { Component, EventEmitter, Input, Output } from '@angular/core';
import { IConfigurationPropertyPayload } from "../../interfaces/configurationPropertyPayload";

@Component({
    selector: 'string-input',
    template: `
    <mat-form-field class="full-width">
        <input matInput [(ngModel)]="csConfig[csDefinition.propertyName]" type="text" placeholder="{{csDefinition.propertyDisplayName}}" pattern="{{csDefinition.validationDefinition.pattern}}" maxlength="{{csDefinition.validationDefinition.maxLength}}" required="{{csDefinition.validationDefinition.isRequired}}">
        <mat-hint>{{csDefinition.propertyDescription}}</mat-hint>
    </mat-form-field>
`})
export class ConfigurationPropertyStringInputComponent {
    @Input()
    public csDefinition: IConfigurationPropertyPayload;
    @Input()
    public csConfig: any;
    @Output()
    public csConfigChange: EventEmitter<string> = new EventEmitter<any>();
}