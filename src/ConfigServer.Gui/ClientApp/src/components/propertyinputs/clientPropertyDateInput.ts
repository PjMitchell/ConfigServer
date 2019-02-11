import { Component, EventEmitter, Input, OnInit, Output, ViewChild } from '@angular/core';
import { FormBuilder, FormControl, ValidatorFn, Validators } from '@angular/forms';
import { Subscription } from 'rxjs';
import { IConfigurationPropertyPayload } from "../../interfaces/configurationPropertyPayload";
import { PropertyInputBase } from './propertyInputBase';

@Component({
    selector: 'date-input',
    template: `
    <mat-form-field class="full-width">
        <input matInput [formControl]="formValue" [matDatepicker]="picker" placeholder="{{placeholder}}" [min]="minValue" [max]="maxValue">
        <mat-datepicker-toggle matSuffix [for]="picker"></mat-datepicker-toggle>
        <mat-datepicker #picker></mat-datepicker>
        <mat-hint *ngIf="csHasInfo">{{csDefinition.propertyDescription}}</mat-hint>
        <mat-error *ngIf="formValue.invalid">{{getErrorMessage()}}</mat-error>
    </mat-form-field>`,
})
export class ConfigurationPropertyDateInputComponent extends PropertyInputBase {

    public formValue = new FormControl();
    public minValue = new Date(-8640000000000000);
    public maxValue = new Date(8640000000000000);

    constructor(formBuilder: FormBuilder) {
        super(formBuilder);
    }

    public getErrorMessage() {
        return this.formValue.hasError('required') ? 'You must enter a value' : '';
    }

    protected getValidators() {
        const validators = new Array<ValidatorFn>();
        if (this._definition.validationDefinition.isRequired) {
            validators.push(Validators.required);
        }
        const min = this._definition.validationDefinition.min;
        if (min) {
            this.minValue = new Date(min as string);
        }
        const max = this._definition.validationDefinition.max;
        if (max) {
            this.maxValue = new Date(max as string);
        }
        return validators;
    }
}
