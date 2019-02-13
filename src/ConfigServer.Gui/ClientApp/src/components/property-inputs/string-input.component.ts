import { Component } from '@angular/core';
import { FormBuilder, ValidatorFn, Validators } from '@angular/forms';
import { PropertyInputBase } from './property-input-base';

@Component({
    selector: 'string-input',
    templateUrl: './string-input.component.html',
})
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
