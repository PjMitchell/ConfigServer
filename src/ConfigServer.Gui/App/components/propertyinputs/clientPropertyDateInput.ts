import { Component, EventEmitter, Input, OnInit, Output, ViewChild } from '@angular/core';
import { IConfigurationPropertyPayload } from "../../interfaces/configurationPropertyPayload";
import { IChildElement } from '../../interfaces/htmlInterfaces';

@Component({
    selector: 'date-input',
    template: `
    <mat-form-field class="full-width">
        <input matInput [(ngModel)]="csConfig[csDefinition.propertyName]" [matDatepicker]="picker" placeholder="{{csDefinition.propertyDisplayName}}" min="{{csDefinition.validationDefinition.min}}" max="{{csDefinition.validationDefinition.max}}" required="{{csDefinition.validationDefinition.isRequired}}">
        <mat-datepicker-toggle matSuffix [for]="picker"></mat-datepicker-toggle>
        <mat-datepicker #picker></mat-datepicker>
        <mat-hint>{{csDefinition.propertyDescription}}</mat-hint>
    </mat-form-field>`,
})
export class ConfigurationPropertyDateInputComponent {
    @Input()
    public csDefinition: IConfigurationPropertyPayload;
    @Input()
    public csConfig: any;
    @Output()
    public csConfigChange: EventEmitter<any> = new EventEmitter<any>();
}
