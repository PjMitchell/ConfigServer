import { Component, EventEmitter, Input, Output } from '@angular/core';
import { IConfigurationPropertyPayload } from "../../interfaces/configurationPropertyPayload";

@Component({
    selector: 'string-input',
    template: `
    <mat-form-field class="full-width">
        <input matInput [(ngModel)]="csConfig[csDefinition.propertyName]" type="text" placeholder="{{placeholder}}" pattern="{{csDefinition.validationDefinition.pattern}}" maxlength="{{csDefinition.validationDefinition.maxLength}}" required="{{csDefinition.validationDefinition.isRequired}}">
        <mat-hint *ngIf="csHasInfo">{{csDefinition.propertyDescription}}</mat-hint>
    </mat-form-field>
`})
export class ConfigurationPropertyStringInputComponent {
    @Input()
    public csDefinition: IConfigurationPropertyPayload;
    @Input()
    public csHasInfo: boolean;
    @Input()
    public csConfig: any;
    @Output()
    public csConfigChange: EventEmitter<string> = new EventEmitter<any>();

    get placeholder(): string {
        if (this.csHasInfo) {
            return this.csDefinition.propertyDisplayName;
        } else {
            return '';
        }
    }
}