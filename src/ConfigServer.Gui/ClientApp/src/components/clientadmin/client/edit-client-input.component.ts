import { Component, EventEmitter, Input, Output } from '@angular/core';
import { IConfigurationClient } from '../../../interfaces/configurationClient';
import { IConfigurationClientGroup } from '../../../interfaces/configurationClientGroup';
import { ITag } from '../../../interfaces/tag';

@Component({
    selector: 'edit-client-input',
    styles: ['./edit-client-input.component.scss'],
    templateUrl: './edit-client-input.component.html',
})
export class EditClientInputComponent {

    @Input()
    public csClient: IConfigurationClient;
    @Output()
    public csClientChange: EventEmitter<IConfigurationClient> = new EventEmitter<IConfigurationClient>();

    @Output()
    public csIsValidChange: EventEmitter<boolean> = new EventEmitter<boolean>();
    @Input()
    public csExistingGroups: IConfigurationClientGroup[];
    @Input()
    public csAvailableTags: ITag[];
    public existingEnviroments: string[];
    private _csIsValid: boolean;
    @Input()
    get csIsValid(): boolean { return this._csIsValid; }
    set csIsValid(value: boolean) {
        this._csIsValid = value;
        this.csIsValidChange.emit(value);
    }

    private _csAllClient: IConfigurationClient[];
    @Input()
    set csAllClient(value: IConfigurationClient[]) {
        this._csAllClient = value;
        if (value) {
            this.existingEnviroments = this.toDistinct(value.map((item) => item.enviroment));
        }
    }

    constructor() {
        this.csExistingGroups = new Array<IConfigurationClientGroup>();
        this.existingEnviroments = new Array<string>();
    }

    public compareTags(t1: ITag, t2: ITag): boolean {
        return t1 && t2 ? t1.value === t2.value : t1 === t2;
    }

    private toDistinct(values: string[]) {
        const set = new Object();
        values.forEach((value) => {
            set[value] = 1;
        });
        const keys = Object.keys(set);
        return keys.filter((value) => value !== "null");
    }
}
