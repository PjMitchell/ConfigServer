import { Component, EventEmitter, Input, Output } from '@angular/core';
import {FormControl, ValidatorFn, Validators} from '@angular/forms';
import { Subscription } from 'rxjs';
import { IConfigurationPropertyPayload } from "../../interfaces/configurationPropertyPayload";

@Component({
    selector: 'float-input',
    template: `
    <mat-form-field class="full-width">
        <input matInput  placeholder="{{placeholder}}" [formControl]="formValue" type="number" step= "0.00001" type= "number"  >
        <mat-hint *ngIf="csHasInfo">{{csDefinition.propertyDescription}}</mat-hint>
        <mat-error *ngIf="formValue.invalid">{{getErrorMessage()}}</mat-error>
    </mat-form-field>
`,
})
export class ConfigurationPropertyFloatInputComponent {
    @Input()
    public csHasInfo: boolean;
    @Output()
    public csConfigChange: EventEmitter<any> = new EventEmitter<any>();
    @Output()
    public onIsValidChanged: EventEmitter<boolean> = new EventEmitter<boolean>();
    public formValue = new FormControl();

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
        return this.formValue.hasError('required') ? 'You must enter a value' :
            this.formValue.hasError('min') ? 'Value must be greater than ' + this._definition.validationDefinition.min :
            this.formValue.hasError('max') ? 'Value must be less than ' + this._definition.validationDefinition.max :
                '';
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
        if (min || min === 0) {
            validators.push(Validators.min(min as number));
        }
        const max = this._definition.validationDefinition.max;
        if (max || max === 0) {
            validators.push(Validators.max(max as number));
        }
        this.formValue.setValidators(validators);
    }
}
