import { Component, EventEmitter, Input, Output } from '@angular/core';
import { FormBuilder, ValidatorFn } from '@angular/forms';
import { IConfigurationPropertyPayload } from "../../interfaces/configurationPropertyPayload";
import { PropertyInputBase } from './propertyInputBase';

@Component({
    selector: 'multiple-option-input',
    template: `
    <mat-form-field class="full-width">
        <mat-select placeholder="{{placeholder}}" [compareWith]="compareFn" [formControl]="formValue" multiple>
            <mat-option *ngFor="let p of csDefinition.options | toKeyValuePairs" [value]="p.key">{{p.value}}</mat-option>
        </mat-select>
        <mat-hint *ngIf="csHasInfo">{{csDefinition.propertyDescription}}</mat-hint>
    </mat-form-field>
`,
})
export class ConfigurationPropertyMultipleOptionInputComponent  extends PropertyInputBase {
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
