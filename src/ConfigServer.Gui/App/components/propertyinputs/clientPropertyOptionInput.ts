﻿import { Component, EventEmitter, Input, Output } from '@angular/core';
import { IConfigurationPropertyPayload } from "../../interfaces/configurationPropertyPayload";

@Component({
    selector: 'option-input',
    template: `
    <mat-form-field class="full-width">
        <mat-select placeholder="{{placeholder}}" [(value)]="csConfig[csDefinition.propertyName]">
            <mat-option *ngFor="let p of csDefinition.options | toKeyValuePairs" [value]="p.key">{{p.value}}</mat-option>
        </mat-select>
        <mat-hint *ngIf="csHasInfo">{{csDefinition.propertyDescription}}</mat-hint>
    </mat-form-field>
`,
})
export class ConfigurationPropertyOptionInputComponent {
    @Input()
    public csDefinition: IConfigurationPropertyPayload;
    @Input()
    public csHasInfo: boolean;
    @Input()
    public csConfig: any;
    @Output()
    public csConfigChange: EventEmitter<any> = new EventEmitter<any>();

    get placeholder(): string {
        if (this.csHasInfo) {
            return this.csDefinition.propertyDisplayName;
        } else {
            return '';
        }
    }
}
