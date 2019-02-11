import { Component, EventEmitter, Input, Output } from '@angular/core';
import { FormBuilder, ValidatorFn } from '@angular/forms';
import { IConfigurationPropertyPayload } from "../../interfaces/configurationPropertyPayload";
import { PropertyInputBase } from './propertyInputBase';

@Component({
    selector: 'bool-input',
    template: `
    <mat-form-field class="full-width">
        <mat-select placeholder="{{placeholder}}" [formControl]="formValue">
            <mat-option [value]="true">True</mat-option>
            <mat-option [value]="false">False</mat-option>
        </mat-select>
        <mat-hint *ngIf="csHasInfo">{{csDefinition.propertyDescription}}</mat-hint>
    </mat-form-field>
`,
})
export class ConfigurationPropertyBoolInputComponent extends PropertyInputBase {
    constructor(formBuilder: FormBuilder) {
        super(formBuilder);
    }
    protected getValidators(): ValidatorFn[] {
        return new Array<ValidatorFn>();
    }

}
