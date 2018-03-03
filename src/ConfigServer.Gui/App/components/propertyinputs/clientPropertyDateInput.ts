import { Component, EventEmitter, Input, OnInit, Output, ViewChild } from '@angular/core';
import { FormControl, ValidatorFn, Validators } from '@angular/forms';
import { Subscription } from 'rxjs';
import { IConfigurationPropertyPayload } from "../../interfaces/configurationPropertyPayload";

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
export class ConfigurationPropertyDateInputComponent {
    @Input()
    public csHasInfo: boolean;
    @Output()
    public csConfigChange: EventEmitter<any> = new EventEmitter<any>();
    @Output()
    public onIsValidChanged: EventEmitter<boolean> = new EventEmitter<boolean>();
    public formValue = new FormControl();
    public minValue = new Date(-8640000000000000);
    public maxValue = new Date(8640000000000000);

    private _definition: IConfigurationPropertyPayload;
    @Input()
    public get csDefinition() {
        return this._definition;
    }
    public set csDefinition(value: IConfigurationPropertyPayload) {
        this._definition = value;
        this.setValidators();
        this.setValue();
    }

    private _config: any;
    @Input()
    public get csConfig() {
        return this._config;
    }
    public set csConfig(value: any) {
        this._config = value;
        this.setValue();
    }

    get placeholder(): string {
        if (this.csHasInfo) {
            return this.csDefinition.propertyDisplayName;
        } else {
            return '';
        }
    }

    private currentValueSubscription: Subscription;
    private currentIsValidSubscription: Subscription;

    public getErrorMessage() {
        return this.formValue.hasError('required') ? 'You must enter a value' : '';
    }

    private setValue() {
        if (!this.csDefinition || !this.csConfig) {
            return;
        }
        if (this.currentValueSubscription) {
            this.currentValueSubscription.unsubscribe();
        }
        this.formValue.setValue(this.csConfig[this.csDefinition.propertyName]);
        this.currentValueSubscription = this.formValue.valueChanges.subscribe((value) => {
            this.csConfig[this.csDefinition.propertyName] = value;
        });
        if (this.currentIsValidSubscription) {
            this.currentIsValidSubscription.unsubscribe();
        }
        this.currentIsValidSubscription = this.formValue.statusChanges.subscribe((value) => {
            this.onIsValidChanged.emit(value === "VALID");
        });
    }
    private setValidators() {
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
        this.formValue.setValidators(validators);
    }
}
