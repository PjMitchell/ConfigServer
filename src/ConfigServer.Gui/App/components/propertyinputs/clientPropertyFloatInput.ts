import { Component, EventEmitter, Input, Output } from '@angular/core';
import { IConfigurationPropertyPayload } from "../../interfaces/configurationPropertyPayload";

@Component({
    selector: 'float-input',
    template: `
    <mat-form-field class="full-width">
        <input matInput [(ngModel)]="csConfig[csDefinition.propertyName]" type="number" placeholder="{{csDefinition.propertyDisplayName}}" step= "0.00001" type= "number" min="{{csDefinition.validationDefinition.min}}" max="{{csDefinition.validationDefinition.max}}" required="{{csDefinition.validationDefinition.isRequired}}">
        <mat-hint>{{csDefinition.propertyDescription}}</mat-hint>
    </mat-form-field>    
`,
})
export class ConfigurationPropertyFloatInputComponent {
    @Input()
    public csDefinition: IConfigurationPropertyPayload;
    @Input()
    public csConfig: any;
    @Output()
    public csConfigChange: EventEmitter<any> = new EventEmitter<any>();
}
