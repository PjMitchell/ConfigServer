import { Component } from '@angular/core';
import { FormBuilder, ValidatorFn, Validators } from '@angular/forms';
import { PropertyInputBase } from './property-input-base';

@Component({
    selector: 'interger-input',
    templateUrl: './interger-input.component.html',
})
export class ConfigurationPropertyIntergerInputComponent extends PropertyInputBase {

    constructor(formBuilder: FormBuilder)  {
        super(formBuilder);
    }

    public getErrorMessage() {
        return this.formValue.hasError('required') ? 'You must enter a value' :
            this.formValue.hasError('min') ? 'Value must be greater than ' + this._definition.validationDefinition.min :
            this.formValue.hasError('max') ? 'Value must be less than ' + this._definition.validationDefinition.max :
                '';
    }

    protected getValidators() {
        const validators = new Array<ValidatorFn>();
        if (this._definition.validationDefinition.isRequired) {
            validators.push(Validators.required);
        }
        const min = this._definition.validationDefinition.min;
        if (min || min === 0) {
            validators.push(Validators.min(min as number));
        }
        const max = this._definition.validationDefinition.max;
        if (max || max === 0) {
            validators.push(Validators.max(max as number));
        }
        return validators;
    }
}
