import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { FormBuilder, FormGroup } from '@angular/forms';
import { IConfigurationPropertyPayload } from "../../interfaces/configurationPropertyPayload";

@Component({
    selector: 'class-input',
    templateUrl: './class-input.component.html',
})
export class ConfigurationPropertyClassInputComponent implements OnInit {
    public classForm: FormGroup;
    public classConfig: any;
    @Input()
    public csDefinition: IConfigurationPropertyPayload;
    @Input()
    public csConfig: any;
    @Input()
    public parentForm: FormGroup;
    @Input()
    public onIsValidChanged: EventEmitter<boolean> = new EventEmitter<boolean>();
    private validation: boolean[] = new Array<boolean>();

    constructor(private formBuilder: FormBuilder) {
    }

    public ngOnInit() {
        if (!this.csDefinition || !this.csConfig || !this.parentForm) {
            return;
        }
        this.classForm = this.formBuilder.group({});
        this.classConfig = this.csConfig[this.csDefinition.propertyName];
        this.parentForm.setControl(this.csDefinition.propertyName, this.classForm);
    }

    private onValidChanged(index: number, isValid: boolean) {
        this.validation[index] = isValid;
        this.onIsValidChanged.emit(this.validation.every((value) => value));
    }
}
