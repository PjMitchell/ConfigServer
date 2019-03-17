import { Component, EventEmitter, Input, Output } from '@angular/core';
import { IConfigurationClientSetting } from '../../../interfaces/configurationClientSetting';

@Component({
    selector: 'edit-clientsetting-input',
    templateUrl: './client-setting-input.component.html',
})
export class EditClientSettingInputComponent {
    @Input()
    public csIsValid: boolean;
    @Output()
    public csIsValidChange: EventEmitter<boolean> = new EventEmitter<boolean>();
    @Input()
    public csSettings: IConfigurationClientSetting[];

    constructor() {
        this.csIsValid = true;
        this.csSettings = new Array<IConfigurationClientSetting>();
    }

    public onKeyChange() {
        this.checkIsValid();
    }

    public add() {
        const newItem: IConfigurationClientSetting = {
            key: 'key',
            value : 'value',
        };
        this.csSettings.push(newItem);
        this.checkIsValid();
    }

    public remove(item: IConfigurationClientSetting) {
        const index = this.csSettings.indexOf(item);
        this.csSettings.splice(index, 1);
        this.checkIsValid();
    }

    private checkIsValid() {
        this.csIsValid = !this.hasDuplicates();
        this.csIsValidChange.emit(this.csIsValid);
    }

    private hasDuplicates() {
        const set = new Object();
        let result = false;
        this.csSettings.forEach((value) => {
            const key = value.key.toUpperCase();
            const count = set[key];
            if (count) {
                result = true;
            } else {
                set[key] = 1;
            }
        });
        return result;
    }
}
