﻿import { Component, Input, Output, EventEmitter } from '@angular/core';
import { ConfigurationClient } from '../interfaces/configurationClient';
import { ConfigurationClientGroup } from '../interfaces/configurationClientGroup';



@Component({
    selector: 'edit-client-input',
    template: `
    <div class="row">
        <div class="col-sm-6 col-md-4">
            <h4>Name:</h4>
            <input [(ngModel)]="csClient.name" type="text">
        </div>
        <div class="col-sm-6 col-md-4">
            <h4>Group:</h4>
            <select class="form-control" [(ngModel)]="csClient.group">
                <option *ngFor="let p of csExistingGroups" [value]="p.groupId">{{p.name}}</option>
            </select>
        </div>
        <div class="col-sm-6 col-md-4">
            <h4>Enviroment:</h4>
            <input [(ngModel)]="csClient.enviroment" type="text" list="enviroments">
            <datalist id="enviroments">
                <option *ngFor="let existingEnviroment of existingEnviroments" value="{{existingEnviroment}}">
            </datalist>
        </div>
        <div class="col-sm-6 col-md-4">
            <h4>Description:</h4>
            <input [(ngModel)]="csClient.description" type="text">
        </div>
`

})
export class EditClientInputComponent {
    @Input()
    csClient: ConfigurationClient;
    @Output()
    csClientChange: EventEmitter<ConfigurationClient> = new EventEmitter<ConfigurationClient>();
    @Input()
    csExistingGroups: ConfigurationClientGroup[]
    existingEnviroments: string[]
    
    private _csAllClient : ConfigurationClient[]
    @Input()
    set csAllClient(value: ConfigurationClient[]) {
        this._csAllClient = value;
        if (value) {
            this.existingEnviroments = this.toDistinct(value.map(item => item.enviroment));
        }
    };

    constructor() {
        this.csExistingGroups = new Array<ConfigurationClientGroup>();
        this.existingEnviroments = new Array<string>();
    }

    private toDistinct(values: string[]) {
        var set = new Object();
        values.forEach((value) => {
            set[value] = 1;
        })
        var keys = Object.keys(set);
        return keys.filter((value)=> value !== "null")
    }
}