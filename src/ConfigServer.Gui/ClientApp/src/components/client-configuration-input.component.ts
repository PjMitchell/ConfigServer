import { Component, EventEmitter, Input, Output } from '@angular/core';
import { FormGroup } from '@angular/forms';
import { IConfigurationModelPayload } from "../interfaces/configurationModelPayload";

@Component({
    selector: 'config-input',
    templateUrl: './client-configuration-input.component.html',
})
export class ConfigurationInputComponent {

    @Input()
    public csModel: IConfigurationModelPayload;
    @Input()
    public csConfig: any;
    @Output()
    public csConfigChange: EventEmitter<any> = new EventEmitter<any>();
    @Output()
    public onIsValidChanged: EventEmitter<boolean> = new EventEmitter<boolean>();
    @Input()
    public parentForm: FormGroup;
    @Output()
    public parentFormChanged: EventEmitter<FormGroup> = new EventEmitter<FormGroup>();

    private validation: boolean[] = new Array<boolean>();
    private onValidChanged(index: number, isValid: boolean) {
        this.validation[index] = isValid;
        this.onIsValidChanged.emit(this.validation.every((value) => value));
    }
}
