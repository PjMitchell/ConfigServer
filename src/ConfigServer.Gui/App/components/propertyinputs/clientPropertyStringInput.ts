import { Component, EventEmitter, Input, Output } from '@angular/core';
import { FormBuilder, FormControl, ValidatorFn, Validators } from '@angular/forms';
import { Subscription } from 'rxjs';
import { IConfigurationPropertyPayload } from "../../interfaces/configurationPropertyPayload";
import { PropertyInputBase } from './propertyInputBase';

@Component({
    selector: 'string-input',
    template: `
    <mat-form-field class="full-width">
        <input matInput placeholder="{{placeholder}}" [formControl]="formValue" type="text">
        <mat-hint *ngIf="csHasInfo">{{csDefinition.propertyDescription}}</mat-hint>
        <mat-error *ngIf="formValue.invalid">{{getErrorMessage()}}</mat-error>
    </mat-form-field>
`})
export class ConfigurationPropertyStringInputComponent  extends PropertyInputBase {

    constructor(formBuilder: FormBuilder) {
        super(formBuilder);
    }

    public getErrorMessage() {
        return this.formValue.hasError('required') ? 'You must enter a value' :
            this.formValue.hasError('maxLength') ? 'Value must be fewer than ' + this._definition.validationDefinition.maxLength + ' char' :
            this.formValue.hasError('pattern') ? 'Value match ' + this._definition.validationDefinition.pattern :
                '';
    }
    protected getValidators() {
        const validators = new Array<ValidatorFn>();
        if (this._definition.validationDefinition.isRequired) {
            validators.push(Validators.required);
        }
        const pattern = this._definition.validationDefinition.pattern;
        if (pattern) {
            validators.push(Validators.pattern(pattern));
        }
        const max = this._definition.validationDefinition.maxLength;
        if (max || max === 0) {
            validators.push(Validators.maxLength(max as number));
        }
        return validators;
    }
}
