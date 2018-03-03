import { Component, EventEmitter, Input, Output } from '@angular/core';
import { FormControl, ValidatorFn, Validators } from '@angular/forms';
import { Subscription } from 'rxjs';
import { IConfigurationPropertyPayload } from "../../interfaces/configurationPropertyPayload";

@Component({
    selector: 'string-input',
    template: `
    <mat-form-field class="full-width">
        <input matInput placeholder="{{placeholder}}" [formControl]="formValue" type="text">
        <mat-hint *ngIf="csHasInfo">{{csDefinition.propertyDescription}}</mat-hint>
        <mat-error *ngIf="formValue.invalid">{{getErrorMessage()}}</mat-error>
    </mat-form-field>
`})
export class ConfigurationPropertyStringInputComponent {
    @Input()
    public csHasInfo: boolean;
    @Output()
    public csConfigChange: EventEmitter<string> = new EventEmitter<any>();
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
            this.formValue.hasError('maxLength') ? 'Value must be fewer than ' + this._definition.validationDefinition.maxLength + ' char' :
            this.formValue.hasError('pattern') ? 'Value match ' + this._definition.validationDefinition.pattern :
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
        const pattern = this._definition.validationDefinition.pattern;
        if (pattern) {
            validators.push(Validators.pattern(pattern));
        }
        const max = this._definition.validationDefinition.maxLength;
        if (max || max === 0) {
            validators.push(Validators.maxLength(max as number));
        }
        this.formValue.setValidators(validators);
    }
}
