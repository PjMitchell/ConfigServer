import { Component, EventEmitter, Input, Output } from '@angular/core';
import { FormBuilder, ValidatorFn } from '@angular/forms';
import { IConfigurationPropertyPayload } from "../../interfaces/configurationPropertyPayload";
import { PropertyInputBase } from './property-input-base';

@Component({
    selector: 'bool-input',
    templateUrl: './bool-input.component.html',
})
export class ConfigurationPropertyBoolInputComponent extends PropertyInputBase {
    constructor(formBuilder: FormBuilder) {
        super(formBuilder);
    }
    protected getValidators(): ValidatorFn[] {
        return new Array<ValidatorFn>();
    }

}
