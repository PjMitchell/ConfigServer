import { Component } from '@angular/core';
import { FormBuilder, FormControl, ValidatorFn, Validators } from '@angular/forms';
import { PropertyInputBase } from './property-input-base';

@Component({
    selector: 'date-input',
    templateUrl: './date-input.component.html',
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
