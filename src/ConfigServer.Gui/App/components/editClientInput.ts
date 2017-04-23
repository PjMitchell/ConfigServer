import { Component, EventEmitter, Input, Output } from '@angular/core';
import { IConfigurationClient } from '../interfaces/configurationClient';
import { IConfigurationClientGroup } from '../interfaces/configurationClientGroup';

@Component({
    selector: 'edit-client-input',
    template: `
    <div class="row">
        <div class="col-sm-6 col-md-4">
            <h4>Name:</h4>
            <input [(ngModel)]="csClient.name" type="text" class="form-control">
        </div>
        <div class="col-sm-6 col-md-4">
            <h4>Group:</h4>
            <select class="form-control" [(ngModel)]="csClient.group">
                <option *ngFor="let p of csExistingGroups" [value]="p.groupId">{{p.name}}</option>
            </select>
        </div>
        <div class="col-sm-6 col-md-4">
            <h4>Enviroment:</h4>
            <input [(ngModel)]="csClient.enviroment" type="text" list="enviroments" class="form-control">
            <datalist id="enviroments">
                <option *ngFor="let existingEnviroment of existingEnviroments" value="{{existingEnviroment}}">
            </datalist>
        </div>
        <div class="col-sm-6 col-md-4">
            <h4>Description:</h4>
            <input [(ngModel)]="csClient.description" type="text" class="form-control">
        </div>
    </div>
    <hr/>
    <edit-clientsetting-input [(csIsValid)]="csIsValid" [csSettings]="csClient.settings"></edit-clientsetting-input>
`,
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

    private toDistinct(values: string[]) {
        const set = new Object();
        values.forEach((value) => {
            set[value] = 1;
        });
        const keys = Object.keys(set);
        return keys.filter((value) => value !== "null");
    }
}
