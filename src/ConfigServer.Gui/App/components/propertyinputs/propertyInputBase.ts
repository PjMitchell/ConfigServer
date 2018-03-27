import { EventEmitter, Input, Output } from "@angular/core";
import { FormBuilder, FormControl, FormGroup, ValidatorFn } from "@angular/forms";
import { Subscription } from "rxjs";
import { IConfigurationPropertyPayload } from "../../interfaces/configurationPropertyPayload";

export abstract class PropertyInputBase {
    @Input()
    public csHasInfo: boolean;

    public formValue = new FormControl();

    protected _definition: IConfigurationPropertyPayload;
    @Input()
    public get csDefinition() {
        return this._definition;
    }
    public set csDefinition(value: IConfigurationPropertyPayload) {
        this._definition = value;
        this.setupForm();
    }

    private _config: any;
    @Input()
    public get csConfig() {
        return this._config;
    }
    public set csConfig(value: any) {
        this._config = value;
        this.setupForm();
    }
    private _parentForm: FormGroup;

    @Input()
    public get parentForm() {
        return this._parentForm;
    }
    public set parentForm(value: FormGroup) {
        this._parentForm = value;
        this.setupForm();
    }

    get placeholder(): string {
        if (this.csHasInfo) {
            return this.csDefinition.propertyDisplayName;
        } else {
            return '';
        }
    }

    private currentValueSubscription: Subscription;

    constructor(private formBuilder: FormBuilder) {

    }
    protected abstract getValidators(): ValidatorFn[];

    private setupForm() {
        if (!this.csDefinition || !this.csConfig || !this.parentForm) {
            return;
        }
        if (this.currentValueSubscription) {
            this.currentValueSubscription.unsubscribe();
        }
        this.formValue = this.formBuilder.control(this.csConfig[this.csDefinition.propertyName], this.getValidators());
        this.formValue.setValue(this.csConfig[this.csDefinition.propertyName]);
        this.currentValueSubscription = this.formValue.valueChanges.subscribe((value) => {
            this.csConfig[this.csDefinition.propertyName] = value;
        });
        this.parentForm.setControl(this.csDefinition.propertyName, this.formValue);
    }

}
