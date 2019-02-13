﻿import { Component } from '@angular/core';
import { FormBuilder, ValidatorFn } from '@angular/forms';
import { PropertyInputBase } from './property-input-base';

@Component({
    selector: 'enum-input',
    templateUrl: './enum-input.component.html',
})
export class ConfigurationPropertyEnumInputComponent extends PropertyInputBase {
    constructor(formBuilder: FormBuilder) {
        super(formBuilder);
    }

    public compareFn(c1: any, c2: any): boolean {
        // tslint:disable-next-line:triple-equals
        return c1 == c2;
    }

    protected getValidators(): ValidatorFn[] {
        return new Array<ValidatorFn>();
    }
}
